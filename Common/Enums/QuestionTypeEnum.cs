using System;

namespace Common.Enums
{
    [Flags]
    public enum QuestionTypeEnum
    {
        None = 0,
        MultipleChoice = 1 << 0,            // שאלה עם מספר אפשרויות בחירה
        FillInTheBlank = 1 << 1,            // שאלה עם מקום להכניס תשובה
        TrueFalse = 1 << 2,                 // נכון/לא נכון
        Ordering = 1 << 3,                  // סידור פריטים בסדר הנכון
        ShortAnswer = 1 << 4,               // תשובה קצרה חופשית
        Listening = 1 << 5,                 // שאלות שמבוססות על האזנה
        ReadingComprehension = 1 << 6,      // הבנת הנקרא
        PictureBased = 1 << 7,              // שאלה מבוססת תמונה
        AudioResponse = 1 << 8,             // תגובה באמצעות הקלטת קול
        ClozeTest = 1 << 9,                 // שאלה עם רווחים בטקסט מורחב
        DragAndDrop = 1 << 10,              // גרירה ושחרור פריטים למקומות
        Pronunciation = 1 << 11,            // בדיקת הגייה
        MultipleAnswer = 1 << 12,           // בחירה של יותר מתשובה אחת
        Spelling = 1 << 13,                 // אימות איות נכון
        ConversationCompletion = 1 << 14,   // השלמת דו-שיח הגיוני בין 2 אפשרויות
        ImageLabeling = 1 << 15,            // תיוג אלמנטים בתמונה
        
        //אם זה מעברית לאנגלית
        Matching = 1 << 16,                 // התאמת זוגות
        Translate = 1 << 17                 // תרגום מילים/משפטים
    }
}
