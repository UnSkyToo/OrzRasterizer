using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using XRasterizer.Base;
using XRasterizer.Helper;

namespace XRasterizer.Core
{
    internal class Rasterizer
    {
        private readonly int CoreIndex_;
        private readonly GraphicsContext Context_;
        private readonly int HalfWidth_;
        private readonly int HalfHeight_;

        public Rasterizer(int CoreIndex, GraphicsContext Context)
        {
            CoreIndex_ = CoreIndex;
            Context_ = Context;
            HalfWidth_ = Context.Viewport.Width / 2;
            HalfHeight_ = Context.Viewport.Height / 2;
        }

        public void Handle(Vertex V1, Vertex V2, Vertex V3)
        {
            if (Context_.FillModeState == FillMode.WireFrame)
            {
                BresenhamLine(V1, V2);
                BresenhamLine(V2, V3);
                BresenhamLine(V3, V1);
            }
            else
            {
                TriangleFill(V1, V2, V3);
            }
        }

        private Fragment ToFragment(Vertex Vert)
        {
            var Frag = new Fragment
            {
                Vert = Vert,
                X = (int)Mathf.Clamp((1 + Vert.Position.X) * HalfWidth_ + 0.5f, 0, Context_.Viewport.Width - 1),
                Y = (int)Mathf.Clamp((1 - Vert.Position.Y) * HalfHeight_ + 0.5f, 0, Context_.Viewport.Height - 1)
            };
            return Frag;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushFragment(Fragment Left, Fragment Right, int X, int Y, float T)
        {
            if (Context_.Viewport.Contanis(X, Y))
            {
                var Z = Mathf.Lerp(Left.Vert.Position.Z, Right.Vert.Position.Z, T);
                var Index = Y * Context_.Viewport.Width + X;
                if (Context_.ZBuffer[Index] > Z)
                {
                    Context_.ZBuffer[Index] = Z;
                    var Frag = new Fragment();
                    Frag.Vert = Vertex.Lerp(Left.Vert, Right.Vert, T);
                    Frag.X = X;
                    Frag.Y = Y;
                    Context_.Fragments.Add(CoreIndex_, Frag);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PushFragment(Vertex Vert, int X, int Y)
        {
            if (Context_.Viewport.Contanis(X, Y))
            {
                var Index = Y * Context_.Viewport.Width + X;
                if (Context_.ZBuffer[Index] > Vert.Position.Z)
                {
                    Context_.ZBuffer[Index] = Vert.Position.Z;
                    var Frag = new Fragment();
                    Frag.Vert = Vert;
                    Frag.X = X;
                    Frag.Y = Y;
                    Context_.Fragments.Add(CoreIndex_, Frag);
                }
            }
        }

        private List<Fragment> SortFragments(params Fragment[] Args)
        {
            var Fragments = new List<Fragment>(Args);
            Fragments.Sort((LHS, RHS) =>
            {
                if (LHS.Y < RHS.Y)
                {
                    return -1;
                }

                if (LHS.Y > RHS.Y)
                {
                    return 1;
                }

                if (LHS.X < RHS.X)
                {
                    return -1;
                }

                if (LHS.X > RHS.X)
                {
                    return 1;
                }
                return 0;
            });

            return Fragments;
        }

        private void TriangleFill(Vertex V1, Vertex V2, Vertex V3)
        {
            var F1 = ToFragment(V1);
            var F2 = ToFragment(V2);
            var F3 = ToFragment(V3);
            var Frags = SortFragments(F1, F2, F3);

            if (Frags[0].Y == Frags[1].Y)
            {
                TriangleTopFill(Frags[0], Frags[1], Frags[2]);
            }
            else if (Frags[1].Y == Frags[2].Y)
            {
                TriangleBottomFill(Frags[1], Frags[2], Frags[0]);
            }
            else
            {
                var T = (double)(Frags[1].Y - Frags[0].Y) / (double)(Frags[2].Y - Frags[0].Y);
                var NewFrag = Fragment.Lerp(Frags[0], Frags[2], T, Frags[1].Y);

                if (NewFrag.X < Frags[1].X)
                {
                    TriangleBottomFill(NewFrag, Frags[1], Frags[0]);
                    TriangleTopFill(NewFrag, Frags[1], Frags[2]);
                }
                else
                {
                    TriangleBottomFill(Frags[1], NewFrag, Frags[0]);
                    TriangleTopFill(Frags[1], NewFrag, Frags[2]);
                }
            }
        }

        private void TriangleTopFill(Fragment Left, Fragment Right, Fragment Bottom)
        {
            if (Bottom.Y == Left.Y)
            {
                ScanlineFill(Left, Right);
            }
            else
            {
                var HeightR = 1.0d / (double) (Bottom.Y - Left.Y);
                for (var Y = Left.Y; Y <= Bottom.Y; ++Y)
                {
                    var T = (double) (Y - Left.Y) * HeightR;

                    var NewLeft = Fragment.Lerp(Left, Bottom, T, Y);
                    var NewRight = Fragment.Lerp(Right, Bottom, T, Y);

                    ScanlineFill(NewLeft, NewRight);
                }
            }
        }

        private void TriangleBottomFill(Fragment Left, Fragment Right, Fragment Top)
        {
            if (Left.Y == Top.Y)
            {
                ScanlineFill(Left, Right);
            }
            else
            {
                var HeightR = 1.0d / (double) (Left.Y - Top.Y);
                for (var Y = Top.Y; Y <= Left.Y; ++Y)
                {
                    var T = (double) (Y - Top.Y) * HeightR;

                    var NewLeft = Fragment.Lerp(Top, Left, T, Y);
                    var NewRight = Fragment.Lerp(Top, Right, T, Y);

                    ScanlineFill(NewLeft, NewRight);
                }
            }
        }

        private void ScanlineFill(Fragment Left, Fragment Right)
        {
            if (Left.X == Right.X)
            {
                PushFragment(Left, Right, Left.X, Left.Y, 0);
                return;
            }
            
            var T = 0.0f;
            var TStep = 1.0f / (float)(Right.X - Left.X);
            for (var X = 0; X <= Right.X - Left.X; ++X)
            {
                PushFragment(Left, Right, Left.X + X, Left.Y, T);
                T += TStep;
            }
        }

        public void BresenhamLine(Vertex Left, Vertex Right)
        {
            var LeftF = ToFragment(Left);
            var RightF = ToFragment(Right);

            var X1 = LeftF.X;
            var Y1 = LeftF.Y;
            var X2 = RightF.X;
            var Y2 = RightF.Y;

            if (!AlgorithmHelper.CohenSutherland.Clip(Context_.Viewport, ref X1, ref Y1, ref X1, ref Y1))
            {
                return;
            }

            var DX = X2 - X1;
            var DY = Y2 - Y1;
            var XInc = DX > 0 ? 1 : -1;
            var YInc = DY > 0 ? 1 : -1;
            var X = X1;
            var Y = Y1;

            DX = Mathf.Abs(DX);
            DY = Mathf.Abs(DY);
            var DeltaX = DX << 1;
            var DeltaY = DY << 1;

            if (DX > DY)
            {
                var WidthR = (float)(1.0d / (double)(X2 - X1));
                var Esp = DeltaY - DX;

                for (var Index = 0; Index <= DX; ++Index)
                {
                    var Vert = Vertex.Lerp(LeftF.Vert, RightF.Vert, (X - X1) * WidthR);
                    PushFragment(Vert, X, Y);

                    if (Esp >= 0)
                    {
                        Esp -= DeltaX;
                        Y += YInc;
                    }

                    Esp += DeltaY;
                    X += XInc;
                }
            }
            else
            {
                var HeightR = (float)(1.0d / (double)(Y2 - Y1));
                var Esp = DeltaX - DY;

                for (var Index = 0; Index <= DY; ++Index)
                {
                    var Vert = Vertex.Lerp(LeftF.Vert, RightF.Vert, (Y - Y1) * HeightR);
                    PushFragment(Vert, X, Y);

                    if (Esp >= 0)
                    {
                        Esp -= DeltaY;
                        X += XInc;
                    }

                    Esp += DeltaX;
                    Y += YInc;
                }
            }
        }
    }
}