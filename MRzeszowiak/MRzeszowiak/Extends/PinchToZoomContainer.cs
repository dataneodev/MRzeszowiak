using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace MRzeszowiak.Extends
{
    public class PinchToZoomContainer : ContentView
    {
        public double currentScale = 1;
        double startScale = 1;
        double xOffset = 0;
        double yOffset = 0;

        double scaleOriginX = 0;
        double scaleOriginY = 0;

        public PinchToZoomContainer()
        {
            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            GestureRecognizers.Add(pinchGesture);
        }

        void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                // Store the current scale factor applied to the wrapped user interface element,
                // and zero the components for the center point of the translate transform.
                startScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
            }
            if (e.Status == GestureStatus.Running)
            {
                // Calculate the scale factor to be applied.
                currentScale += (e.Scale - 1) * startScale;
                currentScale = Math.Max(1, currentScale);

                scaleOriginX = e.ScaleOrigin.X;
                scaleOriginY = e.ScaleOrigin.Y;

                Transform(startScale, currentScale, scaleOriginX, scaleOriginY);
            }
            if (e.Status == GestureStatus.Completed)
            {
                // Store the translation delta's of the wrapped user interface element.
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
            }
        }

        void Transform(double startScale, double targetScale, double ScaleOriginX, double ScaleOriginY)
        {
            // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
            // so get the X pixel coordinate.
            double renderedX = Content.X + xOffset;
            double deltaX = renderedX / Width;
            double deltaWidth = Width / (Content.Width * startScale);
            double originX = (ScaleOriginX - deltaX) * deltaWidth;

            // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
            // so get the Y pixel coordinate.
            double renderedY = Content.Y + yOffset;
            double deltaY = renderedY / Height;
            double deltaHeight = Height / (Content.Height * startScale);
            double originY = (ScaleOriginY - deltaY) * deltaHeight;

            // Calculate the transformed element pixel coordinates.
            double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
            double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

            // Apply translation based on the change in origin.
            Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
            Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

            Content.Scale = currentScale;
        }

        public void ConstTransform(double targetScale)
        {
            startScale = Content.Scale;
            //Content.AnchorX = 0;
            //Content.AnchorY = 0;

            Transform(startScale, targetScale, scaleOriginX, scaleOriginY);

            xOffset = Content.TranslationX;
            yOffset = Content.TranslationY;
        }
    }
}
