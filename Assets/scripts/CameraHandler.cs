using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgcodecsModule;
using System;
using System.Xml;
using System.Linq;
using System.IO;

public class CameraHandler
{
    MatOfDouble m_DistCoeffs = new MatOfDouble();
    Mat m_CameraMatrix = Mat.eye(3, 3, CvType.CV_32F);

    static Dictionary<string, int> m_DistCoeffsIndecies = new Dictionary<string, int>
    {
        {"K1", 0},
        {"K2", 1},
        {"P1", 2},
        {"P2", 3},
        {"K3", 4},
        {"K4", 5},
        {"K5", 6},
        {"K6", 7},
        {"S1", 8},
        {"S2", 9},
        {"S3", 10},
        {"S4", 11},
        {"TX", 12},
        {"TY", 13}
    };

    public Dictionary<string, ImageInfo> Images { get; set; } = new Dictionary<string, ImageInfo>();
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double FocalLength { get; set; }
    //public double SensorSize { get; set; }
    public Point PrincipalPoint { get; set; } = new Point();
    //public double AspectRatio { get; set; } = 1f;
    public Dictionary<string, double> DistCoeffs { get; set; } = new Dictionary<string, double>();

    //由Smart3D参数构建
    public CameraHandler(string name, int width, int height, double focalLength, double sensorSize, Point principalPoint, XmlNode distortionNode)
    {
        Name = name;
        Width = width;
        Height = height;
        //OrigonalFocalLength、sensorSize是mm的，FocalLength、Width、Height是像素的
        FocalLength = focalLength * ((Width > Height) ? Width : Height) / sensorSize;
        PrincipalPoint = principalPoint;
        m_CameraMatrix.put(0, 0, FocalLength, 0, PrincipalPoint.x, 0, FocalLength, PrincipalPoint.y, 0, 0, 1);
        ParseDistortion(distortionNode);
    }

    //由工程文件中的参数或奇正数元参数创建
    //奇正数元内参格式：无畸变，且主点归零
    public CameraHandler(string name, int width, int height, double focalLength, Point principalPoint, XmlNode distortionNode = null)
    {
        Name = name;
        Width = width;
        Height = height;
        FocalLength = focalLength;
        PrincipalPoint = principalPoint;
        m_CameraMatrix.put(0, 0, FocalLength, 0, PrincipalPoint.x, 0, FocalLength, PrincipalPoint.y, 0, 0, 1);

        if (null == distortionNode)
            return;
        ParseDistortion(distortionNode);
    }

    private void ParseDistortion(XmlNode distortionNode)
    {
        double[] distCoeffsList = new double[distortionNode.ChildNodes.Count];
        foreach (XmlNode distCoeff in distortionNode.ChildNodes)
        {
            int index;
            if (m_DistCoeffsIndecies.TryGetValue(distCoeff.Name, out index))
            {
                distCoeffsList[index] = double.Parse(distCoeff.InnerText);
                DistCoeffs.Add(distCoeff.Name, distCoeffsList[index]);
            }
        }
        m_DistCoeffs.fromArray(distCoeffsList);
    }

    public void AddImageInfo(XmlNode photoNode, int width, int height)
    {
        string imagePath = photoNode.SelectSingleNode("ImagePath").InnerText;
        if (Images.ContainsKey(imagePath))
            return;
        Images.Add(
            imagePath,
            new ImageInfo(
            width,
            height,
            new FileInfo(imagePath),
            double.Parse(photoNode.SelectSingleNode("Pose/Rotation/Omega").InnerText),
            double.Parse(photoNode.SelectSingleNode("Pose/Rotation/Phi").InnerText),
            double.Parse(photoNode.SelectSingleNode("Pose/Rotation/Kappa").InnerText),
            double.Parse(photoNode.SelectSingleNode("Pose/Center/x").InnerText),
            double.Parse(photoNode.SelectSingleNode("Pose/Center/y").InnerText),
            double.Parse(photoNode.SelectSingleNode("Pose/Center/z").InnerText)));
    }

    public void AddImageInfo(string line, int width, int height)
    {
        string[] words = System.Text.RegularExpressions.Regex.Split(line, @"\s+");
        string imagePath = words[0];
        if (Images.ContainsKey(imagePath))
            return;
        Images.Add(
            words[0],
            new ImageInfo(
            width,
            height,
            new FileInfo(imagePath),
            double.Parse(words[4]),
            double.Parse(words[5]),
            double.Parse(words[6]),
             double.Parse(words[1]),
            double.Parse(words[2]),
            double.Parse(words[3])));
    }

    public List<ImageInfo> ProjectPoints(Dictionary<int, Vector3> point3Ds, Vector3 faceNormal)
    {
        List<ImageInfo> imageInfos = new List<ImageInfo>();
        List<Point3> cvPoint3Ds = new List<Point3>();
        foreach (var point in point3Ds.Values)
        {
            cvPoint3Ds.Add(new Point3(point.x, point.y, point.z));
        }

        Utils.setDebugMode(true);
        foreach (var image in Images.Values)
        {
            image.DirectionDot = Vector3.Dot(faceNormal, image.Direction);
            image.Index_UVs.Clear();
            MatOfPoint3f cvMatPoint3Ds = new MatOfPoint3f();
            cvMatPoint3Ds.fromList(cvPoint3Ds);
            MatOfPoint2f cvMatPoint2Ds = new MatOfPoint2f();
            image.GiveTranslationVectorDelta();
            Calib3d.projectPoints(cvMatPoint3Ds, image.RotationVector, image.TranslationVector, m_CameraMatrix, m_DistCoeffs, cvMatPoint2Ds);
            List<Point> cvPoint2Ds = cvMatPoint2Ds.toList();
            bool notInThisImage = false;
            for (int i = 0; i < cvPoint2Ds.Count; ++i)
            {
                float u = (float)cvPoint2Ds[i].x / Width;
                float v = 1f - (float)cvPoint2Ds[i].y / Height;
                if (u < 0 || u > 1 || v < 0 || v > 1)
                {
                    notInThisImage = true;
                    break;
                }
                else
                {
                    image.Index_UVs.Add(point3Ds.Keys.ElementAt(i), new Vector2(u, v));
                }
            }
            if (!notInThisImage)
                imageInfos.Add(image);
        }
        Utils.setDebugMode(false);
        return imageInfos;
    }
}
