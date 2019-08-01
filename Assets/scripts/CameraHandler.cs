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

public class CameraHandler
{
    string m_Name;
    int m_Width;
    int m_Height;
    double m_FocalLength;
    double m_SensorSize;
    Point m_PrincipalPoint = new Point();
    MatOfDouble m_DistCoeffs = new MatOfDouble();
    double m_AspectRatio = 1f;
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

    public List<ImageInfo> Images { get; set; } = new List<ImageInfo>();

    public CameraHandler(string name, int width, int height, double focalLength, double sensorSize, Point principalPoint, XmlNode distortionNode, double aspectRatio)
    {
        m_Name = name;
        m_Width = width;
        m_Height = height;
        m_FocalLength = focalLength;
        m_SensorSize = sensorSize;
        m_FocalLength = m_FocalLength * ((m_Width > m_Height) ? m_Width: m_Height) / sensorSize;
        m_PrincipalPoint = principalPoint;
        m_AspectRatio = aspectRatio;
        m_CameraMatrix.put(0, 0, m_FocalLength, 0, m_PrincipalPoint.x, 0, m_FocalLength, m_PrincipalPoint.y, 0, 0, 1);

        double[] distCoeffsList = new double[distortionNode.ChildNodes.Count];
        foreach (XmlNode distCoeff in distortionNode.ChildNodes)
        {
            int index;
            if (m_DistCoeffsIndecies.TryGetValue(distCoeff.Name, out index))
            {
                distCoeffsList[index] = double.Parse(distCoeff.InnerText);
            }
        }
        m_DistCoeffs.fromArray(distCoeffsList); 
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
        foreach (var image in Images)
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
                float u = (float)cvPoint2Ds[i].x / m_Width;
                float v = 1f - (float)cvPoint2Ds[i].y / m_Height;
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
