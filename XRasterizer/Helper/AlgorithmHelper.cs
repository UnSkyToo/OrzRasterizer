using XRasterizer.Base;

namespace XRasterizer.Helper
{
    internal static class AlgorithmHelper
    {
        public static class CohenSutherland
        {
            private const int LeftCode = 0x1;
            private const int RightCode = 0x2;
            private const int BottomCode = 0x4;
            private const int TopCode = 0x8;

            private static int Encode(Box Bounds, int X, int Y)
            {
                var Code = 0;

                if (Y < Bounds.Top)
                {
                    Code |= TopCode;
                }
                else if (Y > Bounds.Bottom)
                {
                    Code |= BottomCode;
                }

                if (X < Bounds.Left)
                {
                    Code |= LeftCode;
                }
                else if (X > Bounds.Right)
                {
                    Code |= RightCode;
                }

                return Code;
            }

            private static void ComputeWithCode(int Code, float X1, float Y1, float X2, float Y2, Box Bounds, out int NewX, out int NewY)
            {
                switch (Code)
                {
                    case 8:
                        NewY = Bounds.Top;
                        NewX = (int)(X1 + (Bounds.Top - Y1) * (X2 - X1) / (Y2 - Y1) + 0.5f);
                        break;
                    case 4:
                        NewY = Bounds.Bottom - 1;
                        NewX = (int)(X1 + (Bounds.Bottom + Y1) * (X2 - X1) / (Y2 - Y1) + 0.5f);
                        break;
                    case 1:
                        NewX = Bounds.Left;
                        NewY = (int)(Y1 + (Bounds.Left - X1) * (Y2 - Y1) / (X2 - X1) + 0.5f);
                        break;
                    case 2:
                        NewX = Bounds.Right - 1;
                        NewY = (int)(Y1 + (Bounds.Right - X1) * (Y2 - Y1) / (X2 - X1) + 0.5f);
                        break;
                    case 9:
                        NewY = Bounds.Top;
                        NewX = (int)(X1 + (Bounds.Top - Y1) * (X2 - X1) / (Y2 - Y1) + 0.5f);

                        if (NewX < Bounds.Left || NewX > Bounds.Right)
                        {
                            NewX = Bounds.Left;
                            NewY = (int)(Y1 + (Bounds.Left - X1) * (Y2 - Y1) / (X2 - X1) + 0.5f);
                        }
                        break;
                    case 10:
                        NewY = Bounds.Top;
                        NewX = (int)(X1 + (Bounds.Top - Y1) * (X2 - X1) / (Y2 - Y1) + 0.5f);

                        if (NewX < Bounds.Left || NewX > Bounds.Right)
                        {
                            NewX = Bounds.Right - 1;
                            NewY = (int)(Y1 + (Bounds.Right - X1) * (Y2 - Y1) / (X2 - X1) + 0.5f);
                        }
                        break;
                    case 6:
                        NewY = Bounds.Bottom - 1;
                        NewX = (int)(X1 + (Bounds.Bottom - Y1) * (X2 - X1) / (Y2 - Y1) + 0.5f);

                        if (NewX < Bounds.Left || NewX > Bounds.Right)
                        {
                            NewX = Bounds.Right - 1;
                            NewY = (int)(Y1 + (Bounds.Right - X1) * (Y2 - Y1) / (X2 - X1) + 0.5f);
                        }
                        break;
                    case 5:
                        NewY = Bounds.Bottom - 1;
                        NewX = (int)(X1 + (Bounds.Bottom - Y1) * (X2 - X1) / (Y2 - Y1) + 0.5f);

                        if (NewX < Bounds.Left || NewX > Bounds.Right)
                        {
                            NewX = Bounds.Left;
                            NewY = (int)(Y1 + (Bounds.Left - X1) * (Y2 - Y1) / (X2 - X1) + 0.5f);
                        }
                        break;
                    default:
                        NewX = 0;
                        NewY = 0;
                        break;
                }
            }

            public static bool Clip(Box Bounds, ref int X1, ref int Y1, ref int X2, ref int Y2)
            {
                var NewX1 = X1;
                var NewY1 = Y1;
                var NewX2 = X2;
                var NewY2 = Y2;

                var P1Code = Encode(Bounds, X1, Y1);
                var P2Code = Encode(Bounds, X2, Y2);

                if ((P1Code & P2Code) == 1)
                {
                    return false;
                }

                if (P1Code == 0 && P2Code == 0)
                {
                    return true;
                }

                if (P1Code != 0)
                {
                    ComputeWithCode(P1Code, X1, Y1, X2, Y2, Bounds, out NewX1, out NewY1);
                }
                if (P2Code != 0)
                {
                    ComputeWithCode(P2Code, X1, Y1, X2, Y2, Bounds, out NewX2, out NewY2);
                }

                if (!Bounds.Contanis(NewX1, NewY1) || !Bounds.Contanis(NewX2, NewY2))
                {
                    return false;
                }
                
                X1 = NewX1;
                Y1 = NewY1;

                X2 = NewX2;
                Y2 = NewY2;

                return true;
            }
        }
    }
}