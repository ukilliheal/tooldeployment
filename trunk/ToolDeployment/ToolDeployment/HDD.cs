using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolDeployment
{
    public class HDD
    {

        public int Index { get; set; }
        public bool IsOK { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Serial { get; set; }
        public Dictionary<int, Smart> Attributes = new Dictionary<int, Smart>() {
                {0x00, new Smart("Invalid")},
                {0x01, new Smart("v Raw read error rate")},
                {0x02, new Smart("^ Throughput performance")},
                {0x03, new Smart("V Spinup time")},
                {0x04, new Smart("  Start/Stop count")},
                {0x05, new Smart("v Reallocated sector count")},//
                {0x06, new Smart("  Read channel margin")},
                {0x07, new Smart("  Seek error rate")},
                {0x08, new Smart("^ Seek timer performance")},
                {0x09, new Smart("  Power-on hours count")},
                {0x0A, new Smart("v Spinup retry count")},//
                {0x0B, new Smart("v Calibration retry count")},
                {0x0C, new Smart("  Power cycle count")},
                {0x0D, new Smart("v Soft read error rate")},
                {0xB8, new Smart("v End-to-End error")},//
                {0xBE, new Smart("v Airflow Temperature")},
                {0xBC, new Smart("v Command Timeout")},//
                {0xBF, new Smart("v G-sense error rate")},
                {0xC0, new Smart("v Power-off retract count")},
                {0xC1, new Smart("v Load/Unload cycle count")},
                {0xC2, new Smart("v HDD temperature")},
                {0xC3, new Smart("  Hardware ECC recovered")},
                {0xC4, new Smart("v Reallocation count")},//
                {0xC5, new Smart("v Current pending sector count")},//
                {0xC6, new Smart("v Uncorrectable Sector Count")},//
                {0xC7, new Smart("v UDMA CRC error rate")},
                {0xC8, new Smart("v Write error rate")},
                {0xC9, new Smart("v Soft read error rate")},//
                {0xCA, new Smart("v Data Address Mark errors")},
                {0xCB, new Smart("v Run out cancel")},
                {0xCC, new Smart("v Soft ECC correction")},
                {0xCD, new Smart("v Thermal asperity rate (TAR)")},
                {0xCE, new Smart("  Flying height")},
                {0xCF, new Smart("v Spin high current")},
                {0xD0, new Smart("  Spin buzz")},
                {0xD1, new Smart("  Offline seek performance")},
                {0xDC, new Smart("v Disk shift")},
                {0xDD, new Smart("v G-sense error rate")},
                {0xDE, new Smart("  Loaded hours")},
                {0xDF, new Smart("  Load/unload retry count")},
                {0xE0, new Smart("v Load friction")},
                {0xE1, new Smart("v Load/Unload cycle count")},
                {0xE2, new Smart("  Load-in time")},
                {0xE3, new Smart("v Torque amplification count")},
                {0xE4, new Smart("v Power-off retract count")},
                {0xE6, new Smart("^ Drive Life Protection Status")},//
                {0xE7, new Smart("v Temperature")},//
                {0xF0, new Smart("  Head flying hours")},
                {0xFA, new Smart("v Read error retry rate")},//
                /* slot in any new codes you find in here */
            };
    }
}
