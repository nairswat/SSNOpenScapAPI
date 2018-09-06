using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OpenScapImpl;
using SSNOpenScap.API.Entities;
using System.Net;

namespace SSNOpenScapAPI.Controllers
{
    [Route("api/docker")]
    public class DockerContainer : Controller
    {
        public static string ip = "xxx.xx.xxx.xxx";
        public static string username = "xxxx";
        public static string password = "xxxxxx";

        WinSCPHelper winSCPHelper = new WinSCPHelper(ip, username, password);
        SSHHelper sshHelper = new SSHHelper(ip, username, password);
        DockerInfo dockerInfo = new DockerInfo();

        List<string> ImageList = new List<string>();
        List<string> ContainerList = new List<string>();

        [HttpPost("machinecredential")]
        public IActionResult PostMachineDetails([FromBody] DockerCredential dc)
        {
            if(dc == null)
            {
                return BadRequest();
            }
            else
            {
                ip = dc.IP;
                username = dc.UserName;
                password = dc.Password;
                return StatusCode(200);
            }
        }

        [HttpGet("images")]
        public IActionResult GetImages()
        {
            string jsondata = sshHelper.executeSSHCommand(dockerInfo.getImagesCommand());
            List<string> StringList = new List<string>();
            StringList = jsondata.Split('\n').ToList();
            StringList = StringList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            Dictionary<string, string> commandDetails = new Dictionary<string, string>();
            ImageList.Clear();
            foreach (string s in StringList)
            {
                //var result = JsonConvert.DeserializeObject(s);
                commandDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(s);
                Images imagedetail = new Images();
                string temp = string.Empty;
                commandDetails.TryGetValue("Repository", out temp);
                imagedetail.ImageName = temp;
                ImageList.Add(JsonConvert.SerializeObject(imagedetail));
                //else if (currentcommand == supportedcommandset.listcontainer) ListContainer.Add(commandDetails);
            }
            //string jsonString = JsonConvert.SerializeObject(ImageList);
            return new JsonResult(ImageList);
        }

        [HttpGet("container")]
        public IActionResult GetContainer()
        {
            string jsondata = sshHelper.executeSSHCommand(dockerInfo.getContainerCommand());
            List<string> StringList = new List<string>();
            StringList = jsondata.Split('\n').ToList();
            StringList = StringList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            Dictionary<string, string> commandDetails = new Dictionary<string, string>();
            ContainerList.Clear();
            foreach (string s in StringList)
            {
                //var result = JsonConvert.DeserializeObject(s);
                commandDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(s);
                Containers imagedetail = new Containers();
                string temp = string.Empty;
                commandDetails.TryGetValue("Names", out temp);
                imagedetail.ContainerName = temp;
                ContainerList.Add(JsonConvert.SerializeObject(imagedetail));
                //else if (currentcommand == supportedcommandset.listcontainer) ListContainer.Add(commandDetails);
            }
            //string jsonString = JsonConvert.SerializeObject(ImageList);
            return new JsonResult(ContainerList);
        }

        [HttpGet("report")]
        [Produces("text/html")]
        public IActionResult GetReport([FromHeader] string containername)
        {
            if (String.IsNullOrEmpty(containername)) containername = "defaultname";
            string commandToExecute = "oscap-docker container-cve " + containername + " --report " + containername + ".html";

            var filename = containername + ".html";
            // Response...
            System.Net.Mime.ContentDisposition cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = false  // false = prompt the user for downloading;  true = browser to try to show the file inline
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");

            return File(System.IO.File.ReadAllBytes(@"D:\download\" + filename), "application/html");

        }
    }
}
