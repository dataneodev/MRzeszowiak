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
        double currentScale = 1;
        double startScale = 1;
        double xOffset = 0;
        double yOffset = 0;
        private const double MAX_SCALE = 4;

        private PanGestureRecognizer panGesture;

        public PinchToZoomContainer()
        {
            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += PinchGesture_PinchUpdated;
            GestureRecognizers.Add(pinchGesture);

            panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);

            var tapGesture = new TapGestureRecognizer();
            tapGesture.NumberOfTapsRequired = 2;
            tapGesture.Tapped += TapGesture_Tapped;
            GestureRecognizers.Add(tapGesture);
        }

        private void TapGesture_Tapped(object sender, EventArgs e)
        {
            double startDScale = 1;
            double midlleDScale = 1.5d;
            double endDScale = 2;

            double gotoScale = startDScale;
            if (currentScale < midlleDScale)
                gotoScale = midlleDScale;
            else if (currentScale < endDScale)
            {
                gotoScale = endDScale;
            }
            else
            {
                gotoScale = startDScale;
            }
            startScale = 1;
            currentScale = gotoScale;

            Content.AnchorX = 0;
            Content.AnchorY = 0;

            double targetX = -(0.5 * Content.Width) * (currentScale - startScale);
            double targetY =  -(0.5 * Content.Height) * (currentScale - startScale);

            // Apply translation based on the change in origin.
            Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
            Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

            // Apply scale factor.
            Content.Scale = currentScale;

            xOffset = Content.TranslationX;
            yOffset = Content.TranslationY;
        }

        private void PinchGesture_PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                panGesture.PanUpdated -= OnPanUpdated;
                // Store the current scale factor applied to the wrapped user interface element,
                // and zero the components for the center point of the translate transform.                
                startScale = Content.Scale;
                Content.AnchorX = 0;
                Content.AnchorY = 0;
            }

            if (e.Status == GestureStatus.Running)
            {
                // Calculate the scale factor to be applied.
                var mesureScale = (e.Scale - 1) * startScale + currentScale;
                if (mesureScale > MAX_SCALE)
                {
                    return;
                }
                currentScale += (e.Scale - 1) * startScale;
                currentScale = Math.Max(1, currentScale);

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the X pixel coordinate.
                double renderedX = Content.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

                // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
                // so get the Y pixel coordinate.
                double renderedY = Content.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * startScale);
                double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

                // Calculate the transformed element pixel coordinates.
                double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

                // Apply translation based on the change in origin.

                Content.TranslationX = targetX.Clamp(-Content.Width * (currentScale - 1), 0);
                Content.TranslationY = targetY.Clamp(-Content.Height * (currentScale - 1), 0);

                // Apply scale factor.
                Content.Scale = currentScale;
            }

            if (e.Status == GestureStatus.Completed)
            {
                panGesture.PanUpdated += OnPanUpdated;
                // Store the translation delta's of the wrapped user interface element.
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
            }
        }

        public void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    Content.AnchorX = 0;
                    Content.AnchorY = 0;
                    break;

                case GestureStatus.Running:
                    // Translate and ensure we don't pan beyond the wrapped user interface element bounds.
                    double scale = Content.Scale;
                    double newX = (e.TotalX * scale) + xOffset;
                    double newY = (e.TotalY * scale) + yOffset;

                    double width = (Content.Width * scale);
                    double height = (Content.Height * scale);

                    double ScreenWidth = Content.Width;
                    double ScreenHeight = Content.Height;

                    bool canMoveX = !(newX < 0 && (newX + width) < ScreenWidth) && !(newX > 0 && (newX + width) > ScreenWidth);
                    bool canMoveY = !(newY < 0 && (newY + height) < ScreenHeight) && !(newY > 0 && (newY + height) > ScreenHeight);

                    if (canMoveX)
                        Content.TranslationX = newX;
                    if (canMoveY)
                        Content.TranslationY = newY;                       
                    break;

                case GestureStatus.Completed:
                    // Store the translation applied during the pan
                    xOffset = Content.TranslationX;
                    yOffset = Content.TranslationY;
                    break;
            }
        }
    }
}
