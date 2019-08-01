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

    private Mat R = new Mat(3, 3, CvType.CV_32F);
    public FileInfo File { get; set; }
    public float DirectionDot { get; set; }
    public Vector3 Direction { get; set; }
    public ImageInfo(FileInfo fileInfo, double omega, double phi, double kappa, double x, double y, double z)
    {
        File = fileInfo;
        m_Omega = (omega + 0) / 180.0 * Math.PI;
        m_Phi = (phi + 0) / 180.0 * Math.PI;
        m_Kappa = (kappa + 0) / 180.0 * Math.PI;
        m_X = x;
        m_Y = y;
        m_Z = z;        
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
        Calib3d.Rodrigues(R, RotationVector);

        Quaternion QuaternionDirection = Quaternion.AngleAxis((float)(omega), Vector3.right) * Quaternion.AngleAxis((float)(phi), Vector3.forward) * Quaternion.AngleAxis((float)(kappa + 0), Vector3.up);
        Direction = (QuaternionDirection * Vector3.down).normalized;
    }

    public void GiveTranslationVectorDelta()
    {
        double x = m_X + SettingsPanelCtrl.Instance.DeltaX;
        double y = m_Y + SettingsPanelCtrl.Instance.DeltaY;
        double z = m_Z;
        Mat C = new Mat(3, 1, CvType.CV_32F);
        C.put(0, 0, x, y, z);
        TranslationVector = -R * C;
    }
}
