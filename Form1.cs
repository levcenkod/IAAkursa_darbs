using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace kursaDarbs
{
    public partial class Form1 : Form
    {
        public ImageClass imageClass = new ImageClass();
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (sender.ToString())
            {
                case "Open":
                    {
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            Image img = Image.FromFile(openFileDialog1.FileName);
                            pictureBox1.Image = img;

                        }
                        break;
                    }
                case "Save":
                    {
                        //jaraksta faila nosaukumu bez formata
                        if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            string filename = saveFileDialog1.FileName;
                            pictureBox1.Image.Save(filename + ".jpg", ImageFormat.Jpeg);
                        }
                        break;
                    }
            }
        }


        private void CompressImageFirstMethod(Image sourceImg)
        {

        }
        private void CompressImageSecondMethod(Image sourceImg)
        {

        }
        private void CompressImageThirdMethod(Image sourceImg)
        {

        }
        private void CompressImageFourthMethod(Image sourceImg)
        {

        }

        public static string ByteArrayToDecimalString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder();
            string format = "{0}";
            foreach (byte b in ba)
            {
                hex.AppendFormat(format, b);
                format = " {0}";
            }
            return hex.ToString();
        }

        public void SaveImgAsText(Bitmap img, string path)
        {
            ImageClass localImageClass = new ImageClass();
            localImageClass.ReadImage(img);
            var rgb = localImageClass.img_original;
            byte[] res = new byte[img.Width*img.Height*3];
            int index = 0;
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    res[index] = rgb[x, y].R;
                    index++;
                    res[index] = rgb[x, y].G;
                    index++;
                    res[index] = rgb[x, y].B;
                    index++;
                }
                
            }
            string result = ByteArrayToDecimalString(res);
            File.WriteAllText(path, result);

        }

        private void firstMethod_Click(object sender, EventArgs e)
        {
            //Image img = pictureBox1.Image;
            //CompressImageFirstMethod(img);
            Bitmap bmp = (Bitmap)pictureBox1.Image.Clone();
            string codePath = "C:\\RTU\\3kurss\\Attelu apstrade\\bitmap.txt";
            string decodePath = "C:\\RTU\\3kurss\\Attelu apstrade\\test.txt";
            string decodedResultPath = "C:\\RTU\\3kurss\\Attelu apstrade\\decoded.txt";
            SaveImgAsText(bmp, codePath);
            saspiest(codePath);
            decode(decodePath);
            string decompressedImageData = File.ReadAllText(decodedResultPath);
            string[] decompressedRGBArray = decompressedImageData.Split(' ');
            byte[] decompressedByteArray = new byte[bmp.Width * bmp.Height * 3];
            for (int i = 0; i < decompressedByteArray.Length; i++)
            {
                decompressedByteArray[i] = Convert.ToByte(decompressedRGBArray[i]);
            }

            PixelRGB[,] decompressedImg = new PixelRGB[bmp.Width, bmp.Height];
            int index = 0;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    decompressedImg[x,y] = new PixelRGB(decompressedByteArray[index], //  0 => R
                                                    decompressedByteArray[index + 1], // +1 => G 
                                                    decompressedByteArray[index + 2]); // +2 => B
                    index = index + 3; 
                }
            }
            pictureBox2.Image = imageClass.DrawImage(decompressedImg);

        }

        private void secondMethod_Click(object sender, EventArgs e)
        {
            Image img = pictureBox1.Image;
            CompressImageSecondMethod(img);
        }

        private void thirdMethod_Click(object sender, EventArgs e)
        {
            Image img = pictureBox1.Image;
            CompressImageThirdMethod(img);
        }

        private void fourthMethod_Click(object sender, EventArgs e)
        {
            Image img = pictureBox1.Image;
            CompressImageFourthMethod(img);
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //first method
        private void saveToFile(String outputFilePath, Dictionary<Char?, int> frequencies, String bits)
        {
            try
            {
                var stream = File.OpenWrite(outputFilePath);
                BinaryWriter os = new BinaryWriter(stream);
                os.Write(frequencies.Count);
                foreach (Char character in frequencies.Keys)
                {
                    os.Write(character);
                    os.Write(frequencies[character]);
                }
                int compressedSizeBits = bits.Length;
                BitArray bitArray = new BitArray(compressedSizeBits);
                for (int i = 0; i < bits.Length; i++)
                {
                    bitArray.set(i, bits.ElementAt(i) != '0' ? 1 : 0);
                }

                os.Write(compressedSizeBits);
                os.Write(bitArray.bytes, 0, bitArray.getSizeInBytes());
                os.Flush();
                os.Close();

            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine(e.ToString());
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }


        private Dictionary<Char?, int> countFrequency(String text)
        {
            Dictionary<Char?, int> freqMap = new Dictionary<Char?, int>();
            for (int i = 0; i < text.Length; i++)
            {
                Char? c = text.ElementAt(i);
                if (freqMap.ContainsKey(c))
                {
                    freqMap[c]++;
                } else
                {
                    freqMap.Add(c, 1);
                }
            }
            return freqMap;
        }
        private CodeTreeNode huffman(List<CodeTreeNode> codeTreeNodes)
        {
            while (codeTreeNodes.Count > 1)
            {
                codeTreeNodes.Sort();

                CodeTreeNode left = (CodeTreeNode)codeTreeNodes[codeTreeNodes.Count - 1];
                codeTreeNodes.RemoveAt(codeTreeNodes.Count - 1);
                CodeTreeNode right = (CodeTreeNode)codeTreeNodes[codeTreeNodes.Count - 1];
                codeTreeNodes.RemoveAt(codeTreeNodes.Count - 1);

                CodeTreeNode parent = new CodeTreeNode(null, right.weight + left.weight, left, right);
                codeTreeNodes.Add(parent);
            }
            return (CodeTreeNode)codeTreeNodes[0];
        }


        private String huffmanDecode(String encoded, CodeTreeNode tree)
        {
            StringBuilder decoded = new StringBuilder();

            CodeTreeNode node = tree;
            for (int i = 0; i < encoded.Length; i++)
            {
                node = encoded.ElementAt(i).Equals('0') ? node.left : node.right;
                if (node.content != null)
                {
                    decoded.Append(node.content);
                    node = tree;
                }
            }
            return decoded.ToString();
        }

        public void saspiest(String filePath)
        {
            try
            {
                String content = File.ReadAllText(filePath).ToString();
                //System.out.println(content);

                Dictionary<Char?, int> frequencies = countFrequency(content);

                //Set<Character> keys = frequencies.keySet();
                //for(Character key : keys){
                // System.out.println( key + "----" + frequencies.get(key) );
                //}

                //System.out.println("Code table:");


                // генерируем список листов дерева
                List<CodeTreeNode> codeTreeNodes = new List<CodeTreeNode>();
                foreach (Char? c in frequencies.Keys)
                {
                    codeTreeNodes.Add(new CodeTreeNode(c, frequencies[c]));
                }

                // строим кодовое дерево с помощью алгоритма Хаффмана

                CodeTreeNode tree = huffman(codeTreeNodes);

                // генерируем таблицу префиксных кодов для кодируемых символов с помощью кодового дерева
                Dictionary<Char?, string> codes = new Dictionary<Char?, string>();
                foreach (Char? c in frequencies.Keys)
                {
                    codes.Add(c, tree.getCodeForCharacter(c, ""));
                }


                StringBuilder encoded = new StringBuilder();
                for (int i = 0; i < content.Length; i++)
                {
                    encoded.Append(codes[content.ElementAt(i)]);
                }
                String compressedtext = "C:\\RTU\\3kurss\\Attelu apstrade\\test.txt";
                saveToFile(compressedtext, frequencies, encoded.ToString());//exports as binary file

            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        // загрузка сжатой информации и таблицы частот из файла
        private void loadFromFile(String inputFilePath, Dictionary<Char?, int> frequencies, StringBuilder bits)
        {
            try
            {
                var stream = File.OpenRead(inputFilePath);
                BinaryReader os = new BinaryReader(stream);
                int frequencyTableSize = os.ReadInt32();
                for (int i = 0; i < frequencyTableSize; i++)
                {
                    frequencies.Add(os.ReadChar(), os.ReadInt32());
                }
                int dataSizeBits = os.ReadInt32();
                BitArray bitArray = new BitArray(dataSizeBits);
                os.Read(bitArray.bytes, 0, bitArray.getSizeInBytes());
                os.Close();

                for (int i = 0; i < bitArray.size; i++)
                {
                    bits.Append(bitArray.get(i) != 0 ? 1 : 0);
                }

            }
            catch (FileNotFoundException e)
            {
                Debug.WriteLine(e.ToString());
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        public void decode(String filename)
        {
            try
            {
                Dictionary<Char?, int> frequencies2 = new Dictionary<Char?, int>();
                StringBuilder encoded2 = new StringBuilder();
                List<CodeTreeNode> codeTreeNodes = new List<CodeTreeNode>();
                // извлечение сжатой информации из файла
                loadFromFile(filename, frequencies2, encoded2);

                // генерация листов и постоение кодового дерева Хаффмана на основе таблицы частот сжатого файла
                foreach (Char? c in frequencies2.Keys)
                {
                    codeTreeNodes.Add(new CodeTreeNode(c, frequencies2[c]));
                }
                CodeTreeNode tree2 = huffman(codeTreeNodes);

                // декодирование обратно исходной информации из сжатой
                String decoded = huffmanDecode(encoded2.ToString(), tree2);


                // сохранение в файл декодированной информации
                String decompressedtext = "C:\\RTU\\3kurss\\Attelu apstrade\\decoded.txt";
                //var stream = File.OpenWrite(decompressedtext);
                //BinaryWriter os = new BinaryWriter(stream);
                //os.Flush();
                //os.Write(decoded);
                //os.Close();
                File.WriteAllText(decompressedtext, decoded);
            }
            catch (IOException e)
            {
                Debug.WriteLine(e.ToString());
            }

        }

    }
}
