using System;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ShadowSXLauncher.Classes;
using ColorPicker = ShadowSXLauncher.UserControls.ColorPicker;

namespace ShadowSXLauncher.Windows;

public partial class ShadowColorAdjuster : Window
{
    private ColorPicker mainColorPicker => GetColorPicker("MainColorPicker");
    private ColorPicker accentColorPicker => GetColorPicker("AccentColorPicker");
    
    private ColorPicker GetColorPicker(string pickerName)
    {
        var colorPicker = this.FindControl<ColorPicker>(pickerName);
        if (colorPicker == null)
        {
            throw new NotImplementedException();
        }

        return colorPicker;
    }
    
    public ShadowColorAdjuster()
    {
        InitializeComponent();
        SetupDefaults();
    }
    
    private void SetupDefaults()
    {
        mainColorPicker.PickerLabel.Text = "Main Color";
        mainColorPicker.SetRGBColor(Color.FromRgb(49,28,16)); //0x311C10
        mainColorPicker.ColorChanged += (sender, args) => { GeneratePreview(); };
        accentColorPicker.PickerLabel.Text = "Accent Color";
        accentColorPicker.SetRGBColor(Color.FromRgb(222,0,0)); //0xDE0000
        accentColorPicker.ColorChanged += (sender, args) => { GeneratePreview(); };
        GeneratePreview();
        ExportButton.Click += OnExportButtonPressed;
        OpenTextureFolderButton.Click += OpenTextureFolderButtonOnClick;
    }

    private void OnExportButtonPressed(object? sender, RoutedEventArgs e)
    {
        string Image1Name = "tex1_128x128_6f6dc295fc576674_6.png";//Playable Shadow Main
        string Image2Name = "tex1_128x128_23470849ed473c96_9bcd3e8f93232964_8.png";//Playable Shadow Eye
        string Image3Name = "tex1_128x128_0a670a23d69f0145_d1f065a85855fcc6_8.png";//Event Shadow Main
        string Image4Name = "tex1_64x64_ad55d92089adb44b_a7fa923a179a8dd1_8.png";//Event Shadow Accent
        string Image5Name = "tex1_128x128_ec9f785c3a033e72_ea84be987812412f_8.png";//Playable Super Shadow Main
        string Image6Name = "tex1_128x128_d690c7dc04b985c8_879d6817d5e22595_9.png";//Event Super Shadow Main
        
        var base1Image = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowColorBase1.png"))));
        var base2Image = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowColorBase2.png"))));
        var base3Image = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowColorBase3.png"))));
        var base4Image = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowColorBase4.png"))));
        var base5Image = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowColorBase5.png"))));
        
        var mask1Image = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowColorMask1.png"))));
        var mask2Image = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowColorMask2.png"))));
        var mask3Image = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowColorMask3.png"))));
        var mask4Image = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowColorMask4.png"))));
        var mask5Image = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowColorMask5.png"))));
        var mask6Image = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowColorMask6.png"))));
        
        // Image newImage = ApplyColorWithMask(true,  baseImage1, colorMask1, AccentColorPreview.BackColor);
        // newImage = ApplyColorWithMask(false,  newImage, colorMask3, MainColorPreview.BackColor);
        var exportImage1 = ApplyColorMaskToBitmap(base1Image, mask1Image, accentColorPicker.GetColor);
        exportImage1 = ApplyColorMaskToBitmap(exportImage1, mask3Image, mainColorPicker.GetColor);
        
        // Image newImage2 = ApplyColorWithMask(true,  baseImage2, colorMask2, AccentColorPreview.BackColor);
        var exportImage2 = ApplyColorMaskToBitmap(base2Image, mask2Image, accentColorPicker.GetColor);
        
        // Image newImage3 = ApplyColorWithMask(false,  baseImage3, colorMask4, MainColorPreview.BackColor);
        // newImage3 = ApplyColorWithMask(true,  newImage3, colorMask5, AccentColorPreview.BackColor);
        var exportImage3 = ApplyColorMaskToBitmap(base3Image, mask4Image, mainColorPicker.GetColor);
        exportImage3 = ApplyColorMaskToBitmap(exportImage3, mask5Image, accentColorPicker.GetColor);
        
        // Image newImage4 = ApplyColorWithMask(true,  baseImage4, null, AccentColorPreview.BackColor);
        var exportImage4 = ApplyColorMaskToBitmap(base4Image, null, accentColorPicker.GetColor);
        
        // Image newImage5 = ApplyColorWithMask(true,  baseImage1, colorMask1, AccentColorPreview.BackColor);
        // newImage5 = ApplyColorWithMask(false,  newImage5, colorMask3, Color.Black);
        var exportImage5 = ApplyColorMaskToBitmap(base1Image, mask1Image, accentColorPicker.GetColor);
        exportImage5 = ApplyColorMaskToBitmap(exportImage5, mask3Image, Colors.Black);
        
        // Image newImage6 = ApplyColorWithMask(true,  baseImage5, colorMask6, AccentColorPreview.BackColor);
        var exportImage6 = ApplyColorMaskToBitmap(base5Image, mask6Image, accentColorPicker.GetColor);
        
        var filePathToShadowTextures = Path.Combine(CommonFilePaths.CustomTexturesPath, "Shadow");
        if (Directory.Exists(filePathToShadowTextures))
        {
            Directory.Delete(filePathToShadowTextures, true);
        }
        Directory.CreateDirectory(filePathToShadowTextures);
        
        SaveBitmapAsPng(exportImage1,filePathToShadowTextures + @"\" + Image1Name);
        SaveBitmapAsPng(exportImage2,filePathToShadowTextures + @"\" + Image2Name);
        SaveBitmapAsPng(exportImage3,filePathToShadowTextures + @"\" + Image3Name);
        SaveBitmapAsPng(exportImage4,filePathToShadowTextures + @"\" + Image4Name);
        SaveBitmapAsPng(exportImage5,filePathToShadowTextures + @"\" + Image5Name);
        SaveBitmapAsPng(exportImage6,filePathToShadowTextures + @"\" + Image6Name);
    }

    private void OpenTextureFolderButtonOnClick(object? sender, RoutedEventArgs e)
    {
        if (Directory.Exists(CommonFilePaths.CustomTexturesPath))
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = CommonFilePaths.GetExplorerPath,
                Arguments = CommonFilePaths.CustomTexturesPath,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
    
    private void GeneratePreview()
    {
        var baseImage = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowPreviewBase.png"))));
        var mainMaskImage = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowPreviewMainMask.png"))));
        var accentMaskImage = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowPreviewAccentMask.png"))));
        
        var preview = ApplyColorMaskToBitmap(baseImage, mainMaskImage, mainColorPicker.GetColor);
        preview = ApplyColorMaskToBitmap(preview, accentMaskImage, accentColorPicker.GetColor);

        PreviewImage.Source = preview;
    }
    
    public void SaveBitmapAsPng(WriteableBitmap bitmap, string filePath)
    {
        // Ensure the file path is valid
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path must be specified.", nameof(filePath));

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            // Use a PngEncoder to save the bitmap
            bitmap.Save(fileStream);
        }
    }

    private WriteableBitmap ApplyColorMaskToBitmap(WriteableBitmap baseImage, WriteableBitmap maskImage, Color colorToApply)
    {
        var newImage  = bitmapToWriteable(baseImage);

        using (var fb = newImage.Lock())
        {
            var buffer = fb.Address;

            for (int y = 0; y < newImage.PixelSize.Height; y++)
            {
                for (int x = 0; x < newImage.PixelSize.Width; x++)
                {
                    var baseColorPixel = GetPixelColor(baseImage, x, y);
                    var newColor = baseColorPixel;
                    var isPixelToColor = (maskImage == null) || GetPixelColor(maskImage, x, y).R > 0;
                    
                    if (isPixelToColor)
                    {
                        newColor = Color.FromRgb((byte)(colorToApply.R * (baseColorPixel.R / 255.0f)),
                                                 (byte)(colorToApply.G * (baseColorPixel.G / 255.0f)),
                                                 (byte)(colorToApply.B * (baseColorPixel.B / 255.0f)));
                    }
                    
                    int pixelIndex = ((y * newImage.PixelSize.Width) + x) * 4;
                    System.Runtime.InteropServices.Marshal.WriteInt32(buffer, pixelIndex, (int)newColor.ToUInt32());
                }
            }
        }

        return newImage;
    }

    private WriteableBitmap bitmapToWriteable(Bitmap bitmap)
    {
        // Convert the Bitmap to WriteableBitmap
        var writeableBitmap = new WriteableBitmap(bitmap.PixelSize, bitmap.Dpi, PixelFormat.Bgra8888);

        using (var lockedBitmap = writeableBitmap.Lock())
        {
            bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), lockedBitmap.Address, (lockedBitmap.RowBytes * lockedBitmap.Size.Height), lockedBitmap.RowBytes);
        }

        // Set the WriteableBitmap to the Image control
        return writeableBitmap;
    }
    
    private Color GetPixelColor(WriteableBitmap bitmap, int x, int y)
    {
        using (var fb = bitmap.Lock())
        {
            var buffer = fb.Address;
            var pixelFormat = bitmap.Format;
            var pixelSize = pixelFormat.Value.BitsPerPixel / 8;
            var stride = fb.RowBytes;
            var offset = (y * stride) + (x * pixelSize);

            unsafe
            {
                byte* ptr = (byte*)buffer.ToPointer() + offset;
                byte b = ptr[0];
                byte g = ptr[1];
                byte r = ptr[2];
                byte a = pixelSize == 4 ? ptr[3] : (byte)255;

                return Color.FromArgb(a, r, g, b);
            }
        }
    }
}