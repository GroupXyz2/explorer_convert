using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FileConverterExtension
{
    public class FFmpegConverter
    {
        public async Task<bool> ConvertFileAsync(string sourceFile, string targetFormat, bool replaceOriginal)
        {
            if (!File.Exists(sourceFile))
            {
                throw new FileNotFoundException("Source file not found.", sourceFile);
            }

            string directory = Path.GetDirectoryName(sourceFile) ?? "";
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFile);
            string tempOutputFile = Path.Combine(directory, $"{fileNameWithoutExtension}_converted.{targetFormat}");
            string finalOutputFile = Path.Combine(directory, $"{fileNameWithoutExtension}.{targetFormat}");

            try
            {
                string arguments = BuildFFmpegArguments(sourceFile, tempOutputFile, targetFormat);

                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();
                    await process.WaitForExitAsync();

                    if (process.ExitCode != 0)
                    {
                        string error = await process.StandardError.ReadToEndAsync();
                        Debug.WriteLine($"FFmpeg error: {error}");
                        return false;
                    }
                }

                if (replaceOriginal)
                {
                    File.Delete(sourceFile);
                    
                    if (File.Exists(finalOutputFile))
                    {
                        File.Delete(finalOutputFile);
                    }
                    File.Move(tempOutputFile, finalOutputFile);
                }
                else
                {
                    if (File.Exists(finalOutputFile))
                    {
                        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        finalOutputFile = Path.Combine(directory, 
                            $"{fileNameWithoutExtension}_{timestamp}.{targetFormat}");
                    }
                    File.Move(tempOutputFile, finalOutputFile);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Conversion error: {ex.Message}");
                
                if (File.Exists(tempOutputFile))
                {
                    try { File.Delete(tempOutputFile); } catch { }
                }
                
                throw;
            }
        }

        private string BuildFFmpegArguments(string inputFile, string outputFile, string targetFormat)
        {
            string escapedInput = $"\"{inputFile}\"";
            string escapedOutput = $"\"{outputFile}\"";

            string baseArgs = $"-i {escapedInput} -y";

            string codecArgs = targetFormat.ToLower() switch
            {
                "mp4" => "-c:v libx264 -preset medium -crf 23 -c:a aac -b:a 192k",
                "mkv" => "-c:v libx265 -preset medium -crf 28 -c:a aac -b:a 192k",
                "webm" => "-c:v libvpx-vp9 -crf 30 -b:v 0 -c:a libopus",
                "avi" => "-c:v mpeg4 -q:v 5 -c:a libmp3lame -b:a 192k",
                
                "mp3" => "-c:a libmp3lame -b:a 320k",
                "aac" => "-c:a aac -b:a 256k",
                "flac" => "-c:a flac",
                "wav" => "-c:a pcm_s16le",
                "ogg" => "-c:a libvorbis -q:a 6",
                "opus" => "-c:a libopus -b:a 128k",
                
                "jpg" or "jpeg" => "-q:v 2",
                "png" => "-compression_level 9",
                "webp" => "-quality 90",
 
                _ => "-c copy"
            };

            return $"{baseArgs} {codecArgs} {escapedOutput}";
        }
    }
}
