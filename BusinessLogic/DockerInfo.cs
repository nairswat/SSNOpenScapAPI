using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenScapImpl
{
    public enum supportedcommandset
    {
        listimage,
        listcontainer,
        none
    }
    class DockerInfo
    {
        supportedcommandset currentcommand = supportedcommandset.none;

        List<Dictionary<string, object>> ListImages = new List<Dictionary<string, object>>();

        List<Dictionary<string, object>> ListContainer = new List<Dictionary<string, object>>();

        public string getImagesCommand()
        {
            currentcommand = supportedcommandset.listimage;
            return "docker images --format \"{{json . }}\"";
        }

        public string getContainerCommand()
        {
            currentcommand = supportedcommandset.listcontainer;
            return "docker ps -a --format \"{{json . }}\"";
        }

        public bool PopulateImages(string jsondata)
        {
            List<string> arrayJson = new List<string>();
            arrayJson = jsondata.Split('\n').ToList();
            arrayJson = arrayJson.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            Dictionary<string, object> commandDetails = new Dictionary<string, object>();
            foreach (string s in arrayJson)
            {
                var result = JsonConvert.DeserializeObject(s);
                commandDetails = (Dictionary<string, object>)(result);
                if (currentcommand == supportedcommandset.listimage) ListImages.Add(commandDetails);
                else if (currentcommand == supportedcommandset.listcontainer) ListContainer.Add(commandDetails); 
            }
            return true;
        }

        public void DisplayDockerImages()
        {
            foreach (Dictionary<string,object> d in ListImages)
            {
                Console.WriteLine("===================================START IMAGES===================================");
                foreach (KeyValuePair<string, object> kvp in d)
                {
                    Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                }
                Console.WriteLine("===================================END IMAGES===================================");
            }
        }

        public void DisplayDockerContainer()
        {
            foreach (Dictionary<string, object> d in ListContainer)
            {
                Console.WriteLine("===================================START CONTAINER===================================");
                foreach (KeyValuePair<string, object> kvp in d)
                {
                    Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                }
                Console.WriteLine("===================================END CONTAINER===================================");
            }
        }
    }
}
