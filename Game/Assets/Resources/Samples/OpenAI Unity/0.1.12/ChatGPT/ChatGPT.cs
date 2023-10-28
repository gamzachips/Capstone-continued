using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Playables;

public enum CharacterType
{
    None,
    William,
    Kinki,
    Cheif,
    Jack,
    Vampire,
    Raskal,
    Ben,
    Claire
}

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;

        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;

        [SerializeField] GameObject withGift;


        private float height;

        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();

        [SerializeField] CharacterType npcType = CharacterType.None;
        public CharacterType NPCType { get { return npcType; } set { npcType = value; } }

        public static string[] likeGrades = { "first meeting(unfamiliar, wary)", "one met, but unfamiliar,wary ", "familiar,Kind. but not friend", "friend,tenderness", "best friend, love", "fucking hate" };
        int likeGrade;

        public string nowState = "talking";

        public static string player_name = "";

        //Gift
        public static string gift_name;
        public static string gift_id;


        public static string[] giftGrades = { "You really hate this.", "You think it's not bad.", "You really love this." };

        public int giftGrade;

        //Prompt
        string prompt_prev = " \n you are the below character, and talking with me . don't use  \"\" or : or (). Keep in mind you are a character with this name. ";

        private string prompt_william =
           "\n\ninstructions: <<you are>>{William. 30 years old. have big farm in town. raise corns these days. short speaker. only have interest in farming. don't like to talk much.} ";

        private string prompt_kinki =
            "\n\ninstructions: <<you are>>{ Kinki. 20 years old. speak in a cute way. a internet streamer(mukbang). have no interest in others. little stupid. feisty to unfamiliar person. love hamburgers. live alone. (never ask me to eat hamburger together)";

        private string prompt_jack =
            "\n\ninstructions: <<you are>>{Jack. 38 years old. a only cook of only restaurant in this town. work all day at restaurant. chatty and kind. love to cook for neighbors. hate to leave food behind. when a customer want to order, they need to open menu in front of the desk. } ";

        private string prompt_raskal =
         "\n\ninstructions: <<you are>>{Raskal. 36 years old. working as a farmer in William's farm with Ben. a friend of Ben. raise corns and cabbages. love music and beer. usually say with 'yeah~' or 'yes~~'.  } ";


        private string prompt_ben =
        "\n\ninstructions: <<you are>>{Ben. 33 years old. working as a farmer in William's farm with Raskal. a friend of Raskal.  raise corns and cabbages. greedy. don't really  trust me. laugh well, but conservative.}";

        private string prompt_cheif =
            "\n\ninstructions: <<you are >>{Robert. 67 years old, a cheif of the town. usually laugh like 'haha'. know well about town. like beer so drinks every day. }"
           + "favorite gifts are steak and egg. worse gifts are cookie, cake and tomato";

        private string prompt_vampire =
            "\n\ninstructions: <<you are>> {name and age is unknown (secret). think yourself as a vampire. go outside only evening and nigth, dawn. elegant way of speaking. laugh like 'ȣȣȣ'";
        private string prompt_claire =
            "\n\ninstructions: <<you are>> {Claire. 46years old. an owner of store in town. buy everything and sell groceries. very kind and lovely woman. didn't marry. store opens 10 to 17. }";

        string prompt_common_info = "\n\n<<Information about this town>> \r\n" +
            "1. ���� �ֹ� : ŲŰ, �ι�Ʈ, �����̾�(�̸� �Ҹ�), ������, ��, ��(��ȭ���)�� �ִ�. " +
            "\r\n2. Kinki: �Թ� ��Ʃ���� �ܹ��Ÿ� �����ϴ� �����̴�. \r\nRobert: �����̴�. \r\n'Vampire':�� �ڽ��� �����̾��� �����ϴ� �̽��͸��� �����̴�." +
            "\r\nWilliam: ������ ��η�, ���ϴ� ���� �����ϴ� ���ڴ�.\r\nJack: ������ ������ �Ĵ��� �丮���, �����̴�. " +
            "\r\n���� ��ȭ����� ���� ������ ���� �������� �ʾҰ� ��縦 �ϴ� û���̴�. " +
            "\r\n3. ���� ��ȭ����� ���� �޸��ų� ��縦 ���ų�, ������ ���� �� �ൿ�� �ϸ� �������� �Ҹ�ȴ�.\r\n�������� �����ϱ� ���ؼ��� �丮�� �ϰų� �Ĵ翡�� �缭 ������ �Ծ�� �Ѵ�. \r\n";

        string prompt_common_last = "\n\nSay only 1~2 Korean sentence. (use within 20 words) Never, Never say English!! only Korean." + "Don't explain before I ask. Talk like real character, not chatGPT. Don't say you will help me. Don't break the instructions.\n";

        string prompt_common_gift;

        private void Start()
        {
            button.onClick.AddListener(SendReply);
        }

        void Update()
        {
            
            // EnterŰ ����
            if (Input.GetKeyDown(KeyCode.Return) && inputField.GetComponent<InputField>().text.Length > 0)
            {
                withGift.SetActive(false);
                SendReply();
            }

        }

        public void UpdateGiftPrompt(string giftId, int grade)
        {
            gift_name = Managers.Data.GetItemData(giftId).name;
            giftGrade = grade;
            gift_id = giftId;
            prompt_common_gift = "\nAnd now you're getting present from me. It is " + gift_name + ", and " + giftGrades[giftGrade] + "Respond to it.=> ";

            //ȣ���� ����
        }

        public void UpdateLikeability()
        {
            likeGrade = gameObject.GetComponent<Likeability>().Grade;

        }

        public void ResetDialogs()
        {
            Transform contentObject = scroll.gameObject.transform.GetChild(0).GetChild(0);
            int size = contentObject.childCount;
            for (int i = 0; i < size; i++)
            {
                Destroy(contentObject.GetChild(i).gameObject);
                messages.Clear();
            }
        }

        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;
            
        }

        private async void SendReply()
        {
            if (prompt_common_gift != "" && gift_id != null)
                gameObject.GetComponent<Likeability>().ChangeWithItem(gift_id, giftGrade);

            gameObject.GetComponent<Likeability>().Increase(0.2f);
            if (messages.Count > 4)
            {
                messages.RemoveAt(0);
                messages.RemoveAt(0);
            }
                

            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            
            AppendMessage(newMessage);
            newMessage.Content = prompt_prev;

            if (messages.Count == 0)
            {
                switch(npcType)
                {
                    case CharacterType.None:
                        break;
                    case CharacterType.William:
                        newMessage.Content += prompt_william;
                        break;
                    case CharacterType.Kinki:
                        newMessage.Content = prompt_kinki;
                        break;
                    case CharacterType.Cheif:
                        newMessage.Content += prompt_cheif;
                        break;
                    case CharacterType.Vampire:
                        newMessage.Content += prompt_vampire;
                        break;
                    case CharacterType.Jack:
                        newMessage.Content += prompt_jack;
                        break;
                    case CharacterType.Raskal:
                        newMessage.Content += prompt_raskal;
                        break;
                    case CharacterType.Ben:
                        newMessage.Content += prompt_ben;
                        break;
                    case CharacterType.Claire:
                        newMessage.Content += prompt_claire;
                        break;
                }
            }

            newMessage.Content += "\nan attitude toward your interlocutor :" + likeGrades[likeGrade] 
                + "\t what you have been doing : " + nowState + prompt_common_gift +"\nmy(who you are talking with) name is :" + Managers.Data.PlayerName +"\n" + prompt_common_last + "Respond to me : "+inputField.text;

            messages.Add(newMessage);
            
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                
                messages.Add(message);
                AppendMessage(message);

            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            button.enabled = true;
            inputField.enabled = true;
            prompt_common_gift = "";
        }
    }
}
