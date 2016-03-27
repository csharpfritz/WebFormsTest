using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fritz.WebFormsTest
{
  public class WebApplicationProxyOptions
  {

    public static readonly WebApplicationProxyOptions DEFAULT = new WebApplicationProxyOptions
    {
      SkipCrawl = true,
      SkipPrecompile = true
    };

    public bool SkipCrawl { get; set; }

    public bool SkipPrecompile { get; set; }

  }

}
