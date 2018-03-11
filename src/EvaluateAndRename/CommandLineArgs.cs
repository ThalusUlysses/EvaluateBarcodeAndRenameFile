using CommandLine;
using CommandLine.Text;

namespace ConsoleApp1
{
    class CommandLineArgs
    {
        public CommandLineArgs()
        {
        }

        [Option('s', "source-dir", HelpText = "Specifiy directory to lookup images for")]
        public string SourceDirectory { get; set; }

        [Option('t', "target-dir", HelpText = "Specifiy directory to write images to")]
        public string TargetDirectory { get; set; }

        [Option('o', "orientation", HelpText = "Specifiy the orientation of the barcode: 0°,90°,180°,270°", DefaultValue = 270)]
        public int? Orientation { get; set; }
        
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }  
}
