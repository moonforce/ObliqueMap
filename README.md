# ObliqueMap - Automatic Texture Mapping Software for Oblique Photogrammetry Models

[中文文档](https://github.com/moonforce/ObliqueMap/blob/master/README_CN.md)

Implemented based on Unity 2018.3.1f1, OpenCV plugin needs to be purchased separately. It supports Smart3D format camera internal and external parameter XML files.

## Quick Start Guide
1. Create a new project.
2. Add an image folder.
3. Add a model file.
4. Add Smart3D data.
5. Set offset parameters.
6. Texture mapping:
   * ​**Automatic mapping**: Click on the "Automatic Mapping" button.
   * ​**Manual mapping**: Set the point size and line thickness, and follow the instructions below.
7. Save the project. The project saves the image path, model path, camera parameters, offset, point size, and line thickness.

https://github.com/user-attachments/assets/9f3b726d-0b7c-4e7b-9661-b3149e39b849

## Table of Contents
- [ObliqueMap - Automatic Texture Mapping Software for Oblique Photogrammetry Models](#obliquemap---automatic-texture-mapping-software-for-oblique-photogrammetry-models)
  - [Quick Start Guide](#quick-start-guide)
  - [Table of Contents](#table-of-contents)
- [1. File](#1-file)
  - [1.1 Create New Project](#11-create-new-project)
  - [1.2 Open Project](#12-open-project)
  - [1.3 Close Project](#13-close-project)
  - [1.4 Save Project](#14-save-project)
  - [1.5 Save Project As](#15-save-project-as)
  - [1.6 Add](#16-add)
    - [1.6.1 Add Aerial Images](#161-add-aerial-images)
    - [1.6.2 Add 3D Model](#162-add-3d-model)
    - [1.6.3 Add Terrain Model](#163-add-terrain-model)
    - [1.6.4 Add Smart3D Aerial Triangulation Data](#164-add-smart3d-aerial-triangulation-data)
  - [1.7 Exit](#17-exit)
- [2. Tools](#2-tools)
  - [2.1 Settings](#21-settings)
    - [2.1.1 Photoshop Path](#211-photoshop-path)
    - [2.1.2 XY Coordinate Offset](#212-xy-coordinate-offset)
    - [2.1.3 Point Diameter](#213-point-diameter)
    - [2.1.4 Line Width](#214-line-width)
  - [2.2 Clear Unused Textures](#22-clear-unused-textures)
  - [2.3 Release Unused Memory](#23-release-unused-memory)
  - [2.4 Clear Camera Parameters](#24-clear-camera-parameters)
  - [2.5 Automatic Mapping](#25-automatic-mapping)
- [3. Help](#3-help)
  - [3.1 Help Documentation](#31-help-documentation)
- [4. Software Operation](#4-software-operation)
  - [4.1 Project Management Tree](#41-project-management-tree)
  - [4.2 Image Window](#42-image-window)
    - [4.2.1 Browse Operations](#421-browse-operations)
    - [4.2.2 UV Adjustment Operations](#422-uv-adjustment-operations)
    - [4.2.3 Right-Click Menu and Shortcut Operations](#423-right-click-menu-and-shortcut-operations)
      - [4.2.3.1 Start Editing (E)](#4231-start-editing-e)
      - [4.2.3.2 Texture Mapping (T)](#4232-texture-mapping-t)
      - [4.2.3.3 View Full Image](#4233-view-full-image)
      - [4.2.3.4 Next Image (Space)](#4234-next-image-space)
      - [4.2.3.5 Cancel Editing (C)](#4235-cancel-editing-c)
  - [4.3 Model Window](#43-model-window)
    - [4.3.1 Browse Operations](#431-browse-operations)
    - [4.3.2 Right-Click Menu and Shortcut Operations](#432-right-click-menu-and-shortcut-operations)
      - [4.3.2.1 Start Editing (E)](#4321-start-editing-e)
      - [4.3.2.2 Open in Photoshop (F)](#4322-open-in-photoshop-f)
      - [4.3.2.3 Refresh Texture (R)](#4323-refresh-texture-r)
      - [4.3.2.4 Clear Texture (D)](#4324-clear-texture-d)
      - [4.3.2.5 Reset Model (R)](#4325-reset-model-r)
      - [4.3.2.6 Export Model (S)](#4326-export-model-s)
      - [4.3.2.7 Cancel Editing (C)](#4327-cancel-editing-c)
  - [4.4 Image Filter Window](#44-image-filter-window)

# 1. File

Project files have the `.omp` extension (Oblique Map Project). `.omp` files are in XML format and store information including image paths, model paths, terrain paths, camera information, and settings panel data.

## 1.1 Create New Project

Create a new `.omp` project. The project name is displayed in the middle of the menu bar. If the project information changes, an `*` is added after the project name as a reminder.

## 1.2 Open Project

Open a `.omp` project. A progress bar indicates the loading progress.

## 1.3 Close Project

Close the current project.

## 1.4 Save Project

Save the current project.

## 1.5 Save Project As

Save the current project as another `.omp` file.

## 1.6 Add

### 1.6.1 Add Aerial Images

Add a folder of aerial images to the project. This operation adds all `.jpg` images in the folder to the project, but does not include images in subfolders. When loading images, the software checks if thumbnails exist in the `thumb` subfolder. If not, it creates them. Note that creating thumbnails is slow, but as long as the thumbnails in the `thumb` folder are not cleared, the same images do not need to be processed again. The purpose of creating thumbnails is to improve the loading speed of the image filter window.

Before adding images, ensure all images are converted to `.dds` files in `r5g6b5` format and placed in the same directory (original `.jpg` files are retained). Using `.dds` files improves the loading speed of the original images.

### 1.6.2 Add 3D Model

Add 3D models. You can select single or multiple 3D model files in the dialog box. Both white models and textured models saved during the mapping process can be added. When loading models, the software checks if UV-complete `.obj` files exist in the `CompleteUvModel` subfolder. If not, it creates them. Creating UV-complete `.obj` files supplements incomplete UV lists in the original white models. As long as the `.obj` files in the `CompleteUvModel` folder are not cleared, the same models do not need to be processed again.

### 1.6.3 Add Terrain Model

Add terrain models in `.obj` format. Ensure the `.obj` files have correct material and texture paths.

### 1.6.4 Add Smart3D Aerial Triangulation Data

The software currently supports Smart3D format camera internal and external parameter XML files.

## 1.7 Exit

Exit the software.

# 2. Tools

## 2.1 Settings

### 2.1.1 Photoshop Path

Set the local Photoshop path for editing extracted textures. You can select the Photoshop `.exe` path using the settings button or directly paste the `.exe` path into the input box. Ensure the path includes the `.exe` filename, e.g., `C:\Program Files\Adobe\Adobe Photoshop CC 2018\Photoshop.exe`.

### 2.1.2 XY Coordinate Offset

Before texture mapping, the vertex coordinates of the white model should be uniformly offset to move them to a smaller coordinate range. Enter the XY offset here. The software adds this offset to the camera's external parameter position information during internal calculations. For example, `x offset = -490000`, `y offset = -3800000`.

### 2.1.3 Point Diameter

Set the diameter of UV points in the image window.

### 2.1.4 Line Width

Set the width of UV lines in the image window.

## 2.2 Clear Unused Textures

During the mapping process, each texture mapping operation generates a `.jpg` file in the same directory as the `.obj` file. However, each face of the model only uses the last generated texture. This button clears unused `.jpg` files, retaining only the `.jpg` files referenced in the material file.

## 2.3 Release Unused Memory

During software operation, memory is recycled when loading and unloading images and models, but memory garbage may still accumulate. This button manually releases memory when memory usage is high.

## 2.4 Clear Camera Parameters

Clear the camera's internal and external parameters.

## 2.5 Automatic Mapping

This feature is not yet implemented.

# 3. Help

## 3.1 Help Documentation

Open this document.

# 4. Software Operation

The software interface is divided into sections as shown below. The dividers between sections can be dragged to resize them.
![UserInterface](https://github.com/moonforce/ObliqueMap/blob/master/UserInterface.jpg)

## 4.1 Project Management Tree

The project management tree is used to manage aerial images, 3D models, and terrain models in the project. Right-click on root nodes to clear or add items; right-click on leaf nodes to delete specific entries. Select multiple leaf nodes and press `Delete` to delete multiple entries simultaneously. Double-click an aerial image to view it in the image window; double-click a 3D model to view it in the model window.

## 4.2 Image Window

### 4.2.1 Browse Operations

Use the mouse wheel to zoom and the middle mouse button to drag.

### 4.2.2 UV Adjustment Operations

Drag UV points by holding the left mouse button over a UV point; drag UV lines by holding the left mouse button over a UV line; move the entire UV area by holding the left mouse button within the UV box but not on a point or line.

### 4.2.3 Right-Click Menu and Shortcut Operations

#### 4.2.3.1 Start Editing (E)

This option is activated when a model is displayed in the model window. Click this button to enter editing mode. In editing mode, the software calculates the projection of the selected face in each image, sorts the images in the image filter window by projection direction, and automatically selects the first image in the image filter window for display in the image window. UV points and lines are also displayed on the image.

#### 4.2.3.2 Texture Mapping (T)

In editing mode, click this button to map the selected UV area to the selected face of the model. This generates a `.jpg` file in the same directory as the model.

#### 4.2.3.3 View Full Image

Click this button to restore the image from a partial view to a full view.

#### 4.2.3.4 Next Image (Space)

In editing mode, click this button to switch to the next image in the image filter window.

#### 4.2.3.5 Cancel Editing (C)

Click this button to exit editing mode.

## 4.3 Model Window

### 4.3.1 Browse Operations

Use the mouse wheel to zoom, the left mouse button to rotate, and the middle mouse button to drag.

### 4.3.2 Right-Click Menu and Shortcut Operations

#### 4.3.2.1 Start Editing (E)

This option is activated when a model is displayed in the model window. Click this button to enter editing mode. In editing mode, the software calculates the projection of the selected face in each image, sorts the images in the image filter window by projection direction, and automatically selects the first image in the image filter window for display in the image window. UV points and lines are also displayed on the image.

#### 4.3.2.2 Open in Photoshop (F)

In editing mode after texture mapping, click this button to open the local `.jpg` file in Photoshop.

#### 4.3.2.3 Refresh Texture (R)

In editing mode, click this button to refresh the texture of the selected face, typically after editing in Photoshop.

#### 4.3.2.4 Clear Texture (D)

In editing mode, click this button to clear the texture of the selected face, returning it to an untextured white state. Note that the `.jpg` file in the model directory is not deleted.

#### 4.3.2.5 Reset Model (R)

Click this button to reset the model to the center of the model window.

#### 4.3.2.6 Export Model (S)

In editing mode, click this button to export the model and update the `.mtl` material file. This operation overwrites the original `.obj` file, so ensure you have a backup.

#### 4.3.2.7 Cancel Editing (C)

Click this button to exit editing mode.

## 4.4 Image Filter Window

In editing mode, drag this window left or right to select images for texture mapping. Double-click an image to send it to the image window.