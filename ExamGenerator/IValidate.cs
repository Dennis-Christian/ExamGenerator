using System.Collections.Generic;

namespace ExamGenerator
{
    public interface IValidate
	{
		List<string> ValidationErrors
		{
			get;
		}
	}
}
