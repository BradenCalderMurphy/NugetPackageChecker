using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NugetPackageChecker {
    public class PackageHandle {

        public PackageHandle(string folderName) {
            if(folderName == null) {
                folderName = "";
            }
            if(!folderName.EndsWith("/") &&
               !folderName.EndsWith("\\")) {
                folderName += "\\";
            }
            this.FolderName = folderName;
        }

        public string FolderName { private set; get; }

        public List<string> GetDiscrepancies() {
            List<string> lst = new List<string>();
            try {
                List<PackageFile> all = new List<PackageFile>();
                foreach (string file in GetPackagesConfigs(this.FolderName)) {
                    all.Add(new PackageFile(file));
                }

                foreach(PackageFile file in all) {
                    lst.AddRange(PackageFile.Differences(file, all.Where(x => !x.Equals(file)).ToList()));
                }
            }
            catch (Exception ex) {
                lst.Clear();
                lst.Add($"ERROR: {ex.Message}");
            }
            return lst;
        }

        private static List<String> GetPackagesConfigs(string folderName) {
            List<String> files = new List<String>();

            if (!Directory.Exists(folderName)) {
                throw new Exception($"Cannot find directory: {folderName}");
            }

            foreach (string f in Directory.GetFiles(folderName, "packages.config")) { 
                files.Add(f);
            }

            foreach (string d in Directory.GetDirectories(folderName)) {
                List<string> dirs = GetPackagesConfigs(d);
                if(dirs == null) {
                    return null;
                }
                files.AddRange(dirs);
            }

            return files;
        }
    }
}
