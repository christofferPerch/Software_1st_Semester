namespace HeartDisease.Utils {
    public static class FileHelper {
        public static string GetMimeType(string fileName) {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch {
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream",
            };
        }

    }
}
