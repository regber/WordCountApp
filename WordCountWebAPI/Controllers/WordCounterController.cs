using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Diagnostics;

namespace WordCountWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordCounter : ControllerBase
    {

        [HttpPost("GetWordDictionary")]
        public string GetWordDictionary([FromBody]string base64Data)
        {
            var wordDictionary = WordCountLibrary.WordCounter.CountingWordsInFileMultThrd(base64Data);

            return WordCountLibrary.Base64.Encode(JsonSerializer.Serialize(wordDictionary));
        }

        [HttpGet("TestConnect")]
        public string TestConnect()
        {
            return "ConnectionSuccessful";
        }


    }
}
