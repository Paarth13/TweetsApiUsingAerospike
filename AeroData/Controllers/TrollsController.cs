using AeroData.Models;
using Aerospike.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AeroData.Controllers
{
    public class TrollsController : ApiController
    {
        // Post: api/Trolls For Displaying a list of Tweets using their tweet id.
        AerospikeClient aerospikeClient = new AerospikeClient("18.235.70.103", 3000);
        string nameSpace = "AirEngine";
        string setName = "Paarth";
        public IEnumerable<Trolls> Post([FromBody]List<string> tweets)
        {

            List<Trolls> tweetData = new List<Trolls>();
            foreach (var tweet in tweets)
            {
                Record record = aerospikeClient.Get(new BatchPolicy(), new Key(nameSpace, setName, tweet));
                Trolls troll = new Trolls();
                if (record != null)
                {
                    troll.tweet_id = record.GetValue("tweet_Id").ToString();
                    troll.content = record.GetValue("Content").ToString();
                }
                else
                {
                    troll.tweet_id = "Object Doesn't exist";
                    troll.content = "Object Doesn't exist";
                }
                tweetData.Add(troll);
            }
            return tweetData;

        }

        // Delete: api/Trolls/    To Delete a Tweet using its Tweet ID
        public void Delete([FromBody]string tweet)
        {
            aerospikeClient.Delete(new WritePolicy(), new Key(nameSpace, setName, tweet));
        }

        // Put: api/Trolls/ Updataing content of a tweet using their tweet id.
        public void Put([FromBody]Trolls trolls)
        {
            aerospikeClient.Put(new WritePolicy(), new Key(nameSpace, setName, trolls.tweet_id), new Bin[] { new Bin("Content", trolls.content) });
        }
    }
}
