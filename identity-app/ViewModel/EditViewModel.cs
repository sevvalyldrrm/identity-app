using System.ComponentModel.DataAnnotations;

namespace identity_app.ViewModel
{
	public class EditViewModel
	{
		public string Id { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string FullName { get; set; } = string.Empty;

		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;

		[DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "Parola eşleşmiyor.")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}
