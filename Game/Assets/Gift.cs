using OpenAI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GiftGrade { Hate, NotBad, Love }


public class Gift : MonoBehaviour
{
    public string[] loveGiftId;
    public string[] hateGiftId;

    public GiftGrade GetGiftGrade(string giftId)
    {
        foreach (string id in loveGiftId)
        {
            if (id == giftId)
            {
                return GiftGrade.Love;
            }

        }
        foreach (string id in hateGiftId)
        {
            if (id == giftId)
            {
                return GiftGrade.Hate;
            }
        }
        return GiftGrade.NotBad;
    }
}
