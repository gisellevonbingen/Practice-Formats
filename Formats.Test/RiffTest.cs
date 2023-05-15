using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Formats.Riff;

namespace Formats.Test
{
    public class RiffTest
    {
        public static void Main()
        {
            var baseDir = @"C:\Users\Seil\Desktop\Test\Riff\";
            var inputDir = $@"{baseDir}Input\";
            var outputDir = $@"{baseDir}Output\";

            foreach (var inputFile in Directory.GetFiles(inputDir, "*", SearchOption.AllDirectories))
            {
                var outputFile = $"{outputDir}{inputFile[inputDir.Length..]}";

                using (var input = new RiffInputStream(new FileStream(inputFile, FileMode.Open)))
                {

                    while (true)
                    {
                        var chunk = input.ReadNextChunk();

                        if (chunk == null)
                        {
                            break;
                        }

                        using (var ms = new MemoryStream())
                        {
                            input.CopyTo(ms);
                            Console.WriteLine($"{input.CurrentPath} : {chunk.TypeKeyToString}, {ms.Length}");
                        }

                    }


                }

                using (var input = new FileStream(inputFile, FileMode.Open))
                {
                    var riff = (RiffChunkFile)RiffChunk.ReadChunk(input);

                    using (var output = new FileStream(outputFile, FileMode.Create))
                    {
                        RiffChunk.WriteChunk(output, riff);
                    }

                }

            }

        }

    }

}
