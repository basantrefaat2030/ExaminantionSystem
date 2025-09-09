namespace ExaminantionSystem.Entities.Enums.Errors
{
    public enum ErrorType
    {
        //course
        COURSE_NOT_FOUND,
        COURSE_TITLE_EXISTS,
        COURSE_HAS_ENROLLMENTS,
        COURSE_HAS_EXAMS,
        COURSE_STATUS_ERROR,
        NOT_COURSE_OWNER,
        INVALID_ENROLLMENT_STATUS,

        //instructor
        INSTRUCTOR_NOT_FOUND,

        // Student errors
        NOT_ENROLLED,
        ENROLLMENT_EXIST,
        ENROLLMENT_NOT_FOUND,
        CANCEL_NOT_ALLOWED,

        // Exam errors
        EXAM_NOT_FOUND,
        EXAM_ALREADY_STARTED,
        FINAL_EXAM_EXISTS,
        EXAM_HAS_ATTEMPTS,
        EXAM_RESULT_NOT_FOUND,
        EXAM_TIME_EXPIRED,


        // Question errors
        QUESTION_NOT_FOUND,
        INVALID_QUESTION_NUMBER,
        QUESTION_IN_USE,
        INVALID_QUESTIONS,
        DUPLICATE_QUESTIONS_IN_REQUEST,


        //Choice errors
        CHOICE_NOT_FOUND,
        INVALID_CORRECT_ANSWERS,
        MIN_CHOICES_REQUIRED,
        MAX_CHOICES_EXCEEDED,
        EMPTY_CHOICE_CONTENT,
        LAST_CORRECT_CHOICE,


        // Access errors
        ACCESS_DENIED,

    }
}
