using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sisbase.Configuration {
    /// <summary>
    /// Interface for creating config files
    /// </summary>
    public interface IConfiguration {
        /// <summary>
        /// The path of the config file
        /// </summary>
        string Path { get; set; }
        /// <summary>
        /// Function that once called will update the config file
        /// </summary>
        void Update();
        /// <summary>
        /// Function that creates a parses an existing config file from a given file or creates a new one if the file doesn't exists.
        /// </summary>
        /// <param name="file"></param>
        void Create(FileInfo file);
    }
}
