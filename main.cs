using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.Serialization.Formatters;
using static System.Formats.Asn1.AsnWriter;
using System.Threading;

public class Program
{
    static void saveImg(int[,] img, string filename, int width, int height)
    {
        Bitmap bmp = new Bitmap(width, height);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                bmp.SetPixel(i, j, Color.FromArgb(img[i, j], img[i, j], img[i, j]));
            }
        }
        bmp.Save(filename, ImageFormat.Bmp);
    }


    /**
    * blackAndWhite
    * Input : a array (dim 3)
    * Output : a array (dim 2)
    * Take a picture in rgb, and returns it in shades of grey
    * 0 = black, and 255 = white. Everyhting in between is grey
    */
    static int[,] BlackAndWhite(Bitmap img, int width, int height)
    {
        int[,] img2 = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                img2[i, j] = (img.GetPixel(i, j).R + img.GetPixel(i, j).B + img.GetPixel(i, j).G) / 3;
            }
        }
        return img2;
    }
    /*
     * InverserContraste
     * Input : an array (dim2), its dimensions (two int)
     * Output : an array (dim2), same dimensions
     * explicit name
     */
    static int[,] InverserContraste(int[,] img, int width, int height)
    {
        int[,] img2 = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                img2[i, j] = 255 - img[i, j];
            }
        }
        return img2;
    }
    /*
     * Contrast
     * Input : an array (dim2), its dimensions (two int)
     * Output : an array (dim2), same dimensions
     * Take a picture that is in shades of grey, and returns it in only black or only white.
     * Everything superior to 240 out of 255 is considered white, and everything else is black
     */
    static int[,] Contrast(int[,] img, int width, int height)
    {
        int[,] img2 = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (img[i, j] > 150)
                {
                    img2[i, j] = 255;
                }
                else
                {
                    img2[i, j] = 0;
                }
            }
        }
        return img2;
    }

    /*
     * Bucket
     * Input : an array (dim2), its dimensions (two int)
     * Output : an array (dim2), same dimensions
     * Take a picture that is black (0) or white (255), and only keep in white the pixels that are reachable from the point 1,1 without crossing black
     */
    static int[,] Bucket(int[,] img, int width, int height)
    {
        int[,] img2 = new int[width, height];


        List<int[]> todolist = new List<int[]>();
        todolist.Add(new int[] { 1, 1 });
        while (todolist.Count != 0)
        {
            if (todolist[0][0] != 0)
            {
                if (todolist[0][0] != width - 1)
                {
                    if (todolist[0][1] != 0)
                    {
                        if (todolist[0][1] != height - 1)
                        {
                            if (img[todolist[0][0] - 1, todolist[0][1]] == 255 & img2[todolist[0][0] - 1, todolist[0][1]] != 255)
                            {
                                img2[todolist[0][0] - 1, todolist[0][1]] = 255;
                                todolist.Add(new int[] { todolist[0][0] - 1, todolist[0][1] });
                            }
                            if (img[todolist[0][0] + 1, todolist[0][1]] == 255 & img2[todolist[0][0] + 1, todolist[0][1]] != 255)
                            {
                                img2[todolist[0][0] + 1, todolist[0][1]] = 255;

                                todolist.Add(new int[] { todolist[0][0] + 1, todolist[0][1] });
                            }
                            if (img[todolist[0][0], todolist[0][1] - 1] == 255 & img2[todolist[0][0], todolist[0][1] - 1] != 255)
                            {
                                img2[todolist[0][0], todolist[0][1] - 1] = 255;

                                todolist.Add(new int[] { todolist[0][0], todolist[0][1] - 1 });
                            }
                            if (img[todolist[0][0], todolist[0][1] + 1] == 255 & img2[todolist[0][0], todolist[0][1] + 1] != 255)
                            {
                                img2[todolist[0][0], todolist[0][1] + 1] = 255;

                                todolist.Add(new int[] { todolist[0][0], todolist[0][1] + 1 });
                            }

                        }
                    }
                }
            }

            todolist.RemoveAt(0);
        }

        return img2;
    }

    static int[,] OnlyOne(int[,] img, int width, int height)
    {
        int[,] img2 = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                img2[i, j] = 255;
            }
        }
        int centerx = width / 2;
        int centery = height / 2;
        int decalage = 0;
        while (img[centerx,centery] == 255)
        {
            if(img[centerx - decalage, centery] == 0)
            {
                centerx -= decalage;
                break;
            }
            if (img[centerx + decalage, centery] == 0)
            {
                centerx += decalage;
                break;
            }
            if (img[centerx , centery - decalage] == 0)
            {
                centery -= decalage;
                break;
            }
            if (img[centerx, centery + decalage] == 0)
            {
                centery += decalage;
                break;
            }
            decalage++;
        }
        
        List<int[]> todolist = new List<int[]>();
        todolist.Add(new int[] { centerx, centery });
        while (todolist.Count != 0)
        {
            if (img2[todolist[0][0] - 1, todolist[0][1]] != 0)
            {
                if (img[todolist[0][0] - 1, todolist[0][1]] == 0)
                {
                    img2[todolist[0][0] - 1, todolist[0][1]] = 0;
                    todolist.Add(new int[] { todolist[0][0] - 1, todolist[0][1] });
                }
            }

            if (img2[todolist[0][0] + 1, todolist[0][1]] != 0)
            {
                if (img[todolist[0][0] + 1, todolist[0][1]] == 0)
                {
                    img2[todolist[0][0] + 1, todolist[0][1]] = 0;
                    todolist.Add(new int[] { todolist[0][0] + 1, todolist[0][1] });
                }
            }

            if (img2[todolist[0][0], todolist[0][1] + 1] != 0)
            {
                if (img[todolist[0][0], todolist[0][1] + 1] == 0)
                {
                    img2[todolist[0][0], todolist[0][1] + 1] = 0;
                    todolist.Add(new int[] { todolist[0][0], todolist[0][1] + 1 });
                }
            }

            if (img2[todolist[0][0], todolist[0][1] - 1] != 0)
            {
                if (img[todolist[0][0], todolist[0][1] - 1] == 0)
                {
                    img2[todolist[0][0], todolist[0][1] - 1] = 0;
                    todolist.Add(new int[] { todolist[0][0], todolist[0][1] - 1 });
                }
            }

            todolist.RemoveAt(0);
        }
        return img2;
    }


    /* Cleaning1
     * Input : an array (dim2), its dimensions (two int)
     * Output : an array (dim2), same dimensions
     * delete black pixels isolated
     */
    static int[,] Cleaning1(int[,] img, int width, int height)
    {
        //we create a second image with the same dimensions, completly white
        int[,] img2 = new int[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                img2[i, j] = 255;

            }
        }
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                if (img[i, j] == 0)
                {


                    if ((img[i, j - 1] == 255) & (img[i, j + 1] == 255) & (img[i - 1, j] == 255) & (img[i + 1, j] == 255))
                    {
                        img2[i, j] = 255;
                    }
                    else
                    {
                        img2[i, j] = 0;
                    }
                }
                else
                {
                    img2[i, j] = 255;
                }
            }
        }
        return img2;
    }


    /* Cleaning2
     * Input : an array (dim2), its dimensions (two int)
     * Output : an array (dim2), same dimensions
     * delete pixels for which two or three neighboor pixels are white
     */

    static int[,] Cleaning2(int[,] img, int width, int height)
    {
        int[,] img2 = new int[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                img2[i, j] = 255;
            }
        }



        for (int i = 1; i < width - 2; i++)
        {
            for (int j = 1; j < height - 2; j++)
            {
                if (img[i, j] == 0)
                {
                    if ((img[i - 1, j] == 255 & img[i, j - 1] == 255 & img[i + 1, j] == 0 & img[i, j + 1] == 0) |
                        (img[i - 1, j] == 255 & img[i, j + 1] == 255 & img[i + 1, j] == 0 & img[i, j - 1] == 0) |
                        (img[i + 1, j] == 255 & img[i, j - 1] == 255 & img[i - 1, j] == 0 & img[i, j + 1] == 0) |
                        (img[i + 1, j] == 255 & img[i, j + 1] == 255 & img[i - 1, j] == 0 & img[i, j - 1] == 0) |

                        (img[i - 1, j] == 255 & img[i, j - 1] == 255 & img[i + 1, j] == 255 & img[i, j + 1] == 0) |
                        (img[i - 1, j] == 255 & img[i, j + 1] == 255 & img[i + 1, j] == 255 & img[i, j - 1] == 0) |
                        (img[i + 1, j] == 255 & img[i, j - 1] == 255 & img[i - 1, j] == 0 & img[i, j + 1] == 255) |
                        (img[i - 1, j] == 255 & img[i, j + 1] == 255 & img[i + 1, j] == 0 & img[i, j - 1] == 255)
                        )
                    {
                        img2[i, j] = 255;
                    }
                    else
                    {
                        img2[i, j] = 0;
                    }
                }
                else
                {
                    img2[i, j] = 255;
                }
            }
        }

        return img2;
    }

    /**
 * Outline
 * Input : an array (dim2)
 * Output : an array (dim2)
 * A function that detect the outline of the tool. It only keeps pixels that have a white neighboor
 */
    static int[,] Outline(int[,] img, int width, int height)
    {
        int[,] img2 = new int[width, height];


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                img2[i, j] = 255;
            }
        }

        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                if (img[i, j] == 0)
                {
                    if ((img[i, j - 1] == 0) & (img[i, j + 1] == 0) & (img[i - 1, j] == 0) & (img[i + 1, j] == 0))
                    {
                        img2[i, j] = 255;
                    }
                    else
                    {
                        img2[i, j] = 0;

                    }

                }
                else
                {
                    img2[i, j] = 255;
                }

            }
        }

        for (int i = 0; i < width; i++)
        {
            img2[i, 0] = 255;
            img2[i, height - 1] = 255;

        }

        for (int i = 0; i < height; i++)
        {
            img2[0, i] = 255;
            img2[width - 1, i] = 255;

        }
        return img2;
    }



    static void Main()
    {
        Bitmap img = new(@"D:\documents\ecole\PI\code contour - v6\ConsoleApp1\0.bmp");

        int width = img.Width;
        int height = img.Height;

        int[,] img0 = BlackAndWhite(img, width, height);
        int[,] img1 = InverserContraste(img0, width, height);
        saveImg(img1, @"D:\documents\ecole\PI\code contour - v6\ConsoleApp1\inversercontraste.bmp", width, height);
        int[,] img2 = Contrast(img1, width, height);
        saveImg(img2, @"D:\documents\ecole\PI\code contour - v6\ConsoleApp1\contraste.bmp", width, height);
        int[,] img3 = Bucket(img2, width, height);
        saveImg(img3, @"D:\documents\ecole\PI\code contour - v6\ConsoleApp1\remplissage.bmp", width, height);
        int[,] img4 = OnlyOne(img3, width, height);
        saveImg(img4, @"D:\documents\ecole\PI\code contour - v6\ConsoleApp1\reduction.bmp", width, height);
        int[,] img5 = Cleaning2(img4, width, height);
        saveImg(img5, @"D:\documents\ecole\PI\code contour - v6\ConsoleApp1\netoyage.bmp", width, height);
        int[,] img6 = Outline(img4, width, height);
        saveImg(img6, @"D:\documents\ecole\PI\code contour - v6\ConsoleApp1\outline.bmp", width, height);

    }

}




