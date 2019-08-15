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
    //新index,uv坐标
    public Dictionary<int, Vector2> Index_UVs { get; set; } = new Dictionary<int, Vector2>();    
    public Mat RotationVector { get; set; } = new Mat(3, 1, CvType.CV_32F);
    public Mat TranslationVector { get; set; } = new Mat(3, 1, CvType.CV_32F);

    private Mat R = new Mat(3, 3, CvType.CV_32F);
    public FileInfo File { get; set; }
    public float DirectionDot { get; set; }
    public Vector3 Direction { get; set; }

    public double Omega { get; set; }
    public double Phi { get; set; }
    public double Kappa { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    public ImageInfo(FileInfo fileInfo, double omega, double phi, double kappa, double x, double y, double z)
    {
        File = fileInfo;
        Omega = omega;
        Phi = phi;
        Kappa = kappa;
        double OmegaRadian = (Omega + 0) / 180.0 * Math.PI;
        double PhiRadian = (Phi + 0) / 180.0 * Math.PI;
        double KappaRadian = (Kappa + 0) / 180.0 * Math.PI;
        X = x;
        Y = y;
        Z = z;        
        R.put(0, 0,
            Math.Cos(PhiRadian) * Math.Cos(KappaRadian),
            Math.Cos(OmegaRadian) * Math.Sin(KappaRadian) + Math.Sin(OmegaRadian) * Math.Sin(PhiRadian) * Math.Cos(KappaRadian),
            Math.Sin(OmegaRadian) * Math.Sin(KappaRadian) - Math.Cos(OmegaRadian) * Math.Sin(PhiRadian) * Math.Cos(KappaRadian),
            -Math.Cos(PhiRadian) * Math.Sin(KappaRadian),
            Math.Cos(OmegaRadian) * Math.Cos(KappaRadian) - Math.Sin(OmegaRadian) * Math.Sin(PhiRadian) * Math.Sin(KappaRadian),
            Math.Sin(OmegaRadian) * Math.Cos(KappaRadian) + Math.Cos(OmegaRadian) * Math.Sin(PhiRadian) * Math.Sin(KappaRadian),
            Math.Sin(PhiRadian),
            -Math.Sin(OmegaRadian) * Math.Cos(PhiRadian),
            Math.Cos(OmegaRadian) * Math.Cos(PhiRadian)
            );        
        Calib3d.Rodrigues(R, RotationVector);

        Quaternion QuaternionDirection = Quaternion.AngleAxis((float)(omega), Vector3.right) * Quaternion.AngleAxis((float)(phi), Vector3.forward) * Quaternion.AngleAxis((float)(kappa + 0), Vector3.up);
        Direction = (QuaternionDirection * Vector3.down).normalized;
    }

    public void GiveTranslationVectorDelta()
    {
        double x = X + SettingsPanelCtrl.Instance.DeltaX;
        double y = Y + SettingsPanelCtrl.Instance.DeltaY;
        double z = Z;
        Mat C = new Mat(3, 1, CvType.CV_32F);
        C.put(0, 0, x, y, z);
        TranslationVector = -R * C;
    }
}
