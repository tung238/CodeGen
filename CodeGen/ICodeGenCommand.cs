﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen
{
    public interface ICodeGenCommand
    {
        string GetSyntaxTree();
        string GetFileName();
        string GetFilePath();
    }
}
