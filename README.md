# ExamGenerator
This software is to used to generate exams from a database of questions and generate pdf output

## Functions
- Questions
  - Create
  - Edit
  - Revisions
  - Multiple choice or free text
 - Exams
   - Create
   - Not editable
   - Notes
   - Create pdf
   - Create pdf correction sheet
  - Exam templates
    - Create
    - Modify
    - Revisions
      - Number of easy/normal/difficult questions
      - Number of multiple choice/free text questions
      - Free text question to back
  - Branding
    - Select colors
    - add company logo

## Why are generated exams read-only?
For legal reasons, exams must be randomly generated and not be editable, 
or else one could critizise the questions that were chosen.

The exams make a note of which questions were used in what revision.

## Exam Generation mockup
```
enum QuestionType 
{
  MultipleChoice,
  FreeText
}

enum QuestionDifficulty
{
  Easy,
  Normal,
  Hard
}

class QuestionSlot
{
  QuestionType Type;
  QuestionDifficulty Difficulty;
  Question Id;
}

var Questions = new QuestionSlot[numQuestions];   //Prepare the Array for the questions
PrepareDifficulties(Questions);                   //Fills slots with QuestionDifficulties
Shuffle(Questions);                               //we need to mix the difficulties and question types
PrepareQuestionTypes(Questions);                  //Fill Slots with QuestionType 
Shuffle(Questions);                               //we need to mix the difficulties and question types
if (FreetextToLast) Sort(Questions);              //Sort by QuestionType (Freetext to back)
Foreach (item in Questions) FindQuestion(item);   //Finds the resulting question and also checks for doubles
```

## Technologies
- WPF for UI
- C# for Code
- MySQL for Database
- https://www.nuget.org/packages/IronPdf for pdf export
