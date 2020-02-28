﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Analogy.Interfaces;

namespace Analogy.LogViewer.XMLLogParser
{
    public static class ChangeLog
    {
        public static IEnumerable<AnalogyChangeLog> GetChangeLog()
        {
            yield return new AnalogyChangeLog("Add Source Link To Project)", AnalogChangeLogType.Improvement, "Lior Banai", new DateTime(2020, 02, 28));
        }
    }
}
