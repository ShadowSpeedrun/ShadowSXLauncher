using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ColorPicker = ShadowSXLauncher.UserControls.ColorPicker;

namespace ShadowSXLauncher.Views;

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
    }
    
    private void GeneratePreview()
    {
        var baseImage = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowPreviewBase.png"))));
        var mainMaskImage = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowPreviewMainMask.png"))));
        var accentMaskImage = bitmapToWriteable(new Bitmap(AssetLoader.Open(new Uri("avares://ShadowSXLauncher/Assets/ShadowColorReferences/ShadowPreviewAccentMask.png"))));

        var preview = bitmapToWriteable(baseImage);

        using (var fb = preview.Lock())
        {
            var buffer = fb.Address;

            for (int y = 0; y < preview.PixelSize.Height; y++)
            {
                for (int x = 0; x < preview.PixelSize.Width; x++)
                {
                    var baseColorPixel = GetPixelColor(baseImage, x, y);
                    var newColor = baseColorPixel;
                    var isMainColorPixel = GetPixelColor(mainMaskImage, x, y).R > 0;
                    var isAccentColorPixel = GetPixelColor(accentMaskImage, x, y).R > 0;

                    
                    if (isMainColorPixel)
                    {
                        newColor = Color.FromRgb((byte)(mainColorPicker.GetColor.R * (baseColorPixel.R / 255.0f)),
                                                 (byte)(mainColorPicker.GetColor.G * (baseColorPixel.G / 255.0f)),
                                                 (byte)(mainColorPicker.GetColor.B * (baseColorPixel.B / 255.0f)));
                    }

                    if (isAccentColorPixel)
                    {
                        newColor = Color.FromRgb((byte)(accentColorPicker.GetColor.R * (baseColorPixel.R / 255.0f)),
                            (byte)(accentColorPicker.GetColor.G * (baseColorPixel.G / 255.0f)),
                            (byte)(accentColorPicker.GetColor.B * (baseColorPixel.B / 255.0f)));
                    }
                    
                    int pixelIndex = ((y * preview.PixelSize.Width) + x) * 4;
                    System.Runtime.InteropServices.Marshal.WriteInt32(buffer, pixelIndex, (int)newColor.ToUInt32());
                }
            }
        }

        PreviewImage.Source = (Bitmap)preview;
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