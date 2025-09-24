using Google.GData.Extensions;
using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Reader", menuName = "ScriptableObject/Item/DataReader")]
public class ItemDataReader : DataReaderBase
{
    [Header("스프레드시트에서 읽혀져 직렬화 된 오브젝트")][SerializeField] public List<ItemData> DataList = new List<ItemData>();

    public async Task<Texture2D> GetIcon(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogWarning("URL이 비어있습니다.");
            return null;
        }

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);

        try
        {
            // await 사용을 위해 확장 메서드 필요
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"이미지 다운로드 실패: {www.error} - URL: {url}");
                return null;
            }

            var texture = (DownloadHandlerTexture)www.downloadHandler;
            
            return texture.texture;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"이미지 다운로드 예외: {e.Message}");
            return null;
        }
        finally
        {
            // 메모리 누수 방지
            www?.Dispose();
        }
    }

    internal async Task UpdateStats(List<GSTU_Cell> list, int itemID)
    {
        int Id = 0;
        string Name = "";
        ItemStatType Ability_1Type = ItemStatType.none;
        int Ability_1Value = 0;
        ItemStatType Ability_2Type = ItemStatType.none;
        int Ability_2Value = 0;
        int ItemTalentId = 0;
        int SynergyId = 0;
        Texture2D Icon = null;
        ItemGradeType ItemGradeType = ItemGradeType.common;
        int DescriptionId = 0;

        for (int i = 0; i < list.Count; i++)
        {
            switch (list[i].columnId)
            {
                case "ID":
                    {
                        Id = int.Parse(list[i].value);
                        break;
                    }
                case "Grade":
                    {
                        ItemGradeType = Enum.Parse<ItemGradeType>(list[i].value);
                        break;
                    }

                case "Name":
                    {
                        Name = list[i].value;
                        break;
                    }
                case "Ability_1":
                    {
                        Ability_1Type = Enum.Parse<ItemStatType>(list[i].value);
                        break;
                    }
                case "Value_1":
                    {
                        Ability_1Value = int.Parse(list[i].value);
                        break;
                    }
                case "Ability_2":
                    {
                        Ability_2Type = Enum.Parse<ItemStatType>(list[i].value);
                        break;
                    }
                case "Value_2":
                    {
                        Ability_2Value = int.Parse(list[i].value);
                        break;
                    }
                case "ItemTalent_ID":
                    {
                        ItemTalentId = int.Parse(list[i].value);
                        break;
                    }
                case "Synergy_ID":
                    {
                        SynergyId = int.Parse(list[i].value);
                        break;
                    }
                case "IconRoute":
                    {
                        if (list[i].value == "-")
                            break;
                        Debug.Log(list[i].value);
                        Icon = await GetIcon(list[i].value);
                        if (Icon == null)
                        {
                            Debug.Log("why null");
                        }
                        else
                        {

                            Debug.Log("why not null");
                        }
                        break;
                    }
                case "Desc":
                    {
                        DescriptionId = int.Parse(list[i].value);
                        break;
                    }
            }
        }

        DataList.Add(new ItemData(Id, ItemGradeType, Name, Ability_1Type, Ability_1Value, Ability_2Type, Ability_2Value, ItemTalentId, SynergyId, DescriptionId, Icon));
    }
}
