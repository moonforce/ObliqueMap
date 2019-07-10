using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.UnityUtils;
using System.IO;
using System;

public class ImageInfo
{
    static double DeltaX = -490000;
    static double DeltaY = -3800000;

    double m_Omega;//x
    double m_Phi;//y
    double m_Kappa;//z
    double m_X;
    double m_Y;
    double m_Z;

    //新index,uv坐标
    public Dictionary<int, Vector2> Index_UVs { get; set; } = new Dictionary<int, Vector2>();    
    public Mat RotationVector { get; set; } = new Mat(3, 1, CvType.CV_32F);
    public Mat TranslationVector { get; set; } = new Mat(3, 1, CvType.CV_32F);
    public FileInfo File { get; set; }

    public ImageInfo(FileInfo fileInfo, double omega, double phi, double kappa, double x, double y, double z)
    {
        File = fileInfo;
        m_Omega = (omega + 0) / 180.0 * Math.PI;
        m_Phi = (phi + 0) / 180.0 * Math.PI;
        m_Kappa = (kappa + 0) / 180.0 * Math.PI;
        m_X = x + DeltaX;
        m_Y = y + DeltaY;
        m_Z = z;
        Mat R = new Mat(3, 3, CvType.CV_32F);
        Mat C = new Mat(3, 1, CvType.CV_32F);
        C.put(0, 0, m_X, m_Y, m_Z);
        R.put(0, 0,
            Math.Cos(m_Phi) * Math.Cos(m_Kappa),
            Math.Cos(m_Omega) * Math.Sin(m_Kappa) + Math.Sin(m_Omega) * Math.Sin(m_Phi) * Math.Cos(m_Kappa),
            Math.Sin(m_Omega) * Math.Sin(m_Kappa) - Math.Cos(m_Omega) * Math.Sin(m_Phi) * Math.Cos(m_Kappa),
            -Math.Cos(m_Phi) * Math.Sin(m_Kappa),
            Math.Cos(m_Omega) * Math.Cos(m_Kappa) - Math.Sin(m_Omega) * Math.Sin(m_Phi) * Math.Sin(m_Kappa),
            Math.Sin(m_Omega) * Math.Cos(m_Kappa) + Math.Cos(m_Omega) * Math.Sin(m_Phi) * Math.Sin(m_Kappa),
            Math.Sin(m_Phi),
            -Math.Sin(m_Omega) * Math.Cos(m_Phi),
            Math.Cos(m_Omega) * Math.Cos(m_Phi)
            );
        TranslationVector = -R * C;
        Calib3d.Rodrigues(R, RotationVector);
    }
}
