# SMTP Configuration Guide

## Quick Setup

To enable email functionality for password reset, add this configuration to `Web.config`:

```xml
<configuration>
  <!-- Add this section inside <configuration> -->
  <system.net>
    <mailSettings>
      <smtp from="noreply@thuviendhnhatrang.edu.vn">
        <network 
          host="smtp.gmail.com" 
          port="587" 
          userName="your-email@gmail.com" 
          password="your-app-password" 
          enableSsl="true" />
      </smtp>
    </mailSettings>
  </system.net>
  
  <!-- Add this to existing <appSettings> -->
  <appSettings>
    <add key="EmailFrom" value="noreply@thuviendhnhatrang.edu.vn" />
  </appSettings>
</configuration>
```

## Gmail Setup (Recommended for Testing)

### Step 1: Enable 2-Factor Authentication
1. Go to https://myaccount.google.com/
2. Navigate to **Security** section
3. Enable **2-Step Verification**

### Step 2: Generate App Password
1. Still in Security section, find **App passwords**
2. Select app: **Mail**
3. Select device: **Windows Computer**
4. Click **Generate**
5. Copy the 16-character password (xxxx xxxx xxxx xxxx)

### Step 3: Update Web.config
Replace `your-email@gmail.com` and `your-app-password`:

```xml
<network 
  host="smtp.gmail.com" 
  port="587" 
  userName="library@gmail.com" 
  password="abcd efgh ijkl mnop" 
  enableSsl="true" />
```

## Alternative SMTP Providers

### Option 1: Office 365 / Outlook

```xml
<network 
  host="smtp.office365.com" 
  port="587" 
  userName="your-email@outlook.com" 
  password="your-password" 
  enableSsl="true" />
```

### Option 2: SendGrid (Production Recommended)

```xml
<network 
  host="smtp.sendgrid.net" 
  port="587" 
  userName="apikey" 
  password="your-sendgrid-api-key" 
  enableSsl="true" />
```

### Option 3: Local SMTP Server

```xml
<network 
  host="localhost" 
  port="25" 
  enableSsl="false" />
```

## Testing Email Functionality

### Test 1: Simple Test (Optional)

Create a test page to verify SMTP works:

```csharp
using System.Net.Mail;
using System.Net;

// In a controller action:
public ActionResult TestEmail()
{
    try
    {
        using (SmtpClient smtp = new SmtpClient())
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("noreply@thuviendhnhatrang.edu.vn");
            mail.To.Add("your-test-email@gmail.com");
            mail.Subject = "Test Email";
            mail.Body = "If you receive this, SMTP is configured correctly!";
            
            smtp.Send(mail);
        }
        return Content("Email sent successfully!");
    }
    catch (Exception ex)
    {
        return Content("Error: " + ex.Message);
    }
}
```

### Test 2: Password Reset Flow

1. Go to user management: `/Admin_65131559/NguoiDung_65131559`
2. Find a user with valid email
3. Click the 🔑 (key) icon
4. Check email inbox (and spam folder)
5. Verify reset email was received

## Troubleshooting

### Problem: "The SMTP server requires a secure connection"

**Solution**: Ensure `enableSsl="true"` in configuration

### Problem: "Authentication failed"

**Solutions**:
- Verify username and password are correct
- For Gmail: Use App Password, not regular password
- For Office365: Enable SMTP AUTH in admin settings

### Problem: "Mailbox unavailable"

**Solution**: Verify the `from` email address is valid

### Problem: Emails go to spam

**Solutions**:
- Use a verified domain email (not @gmail.com)
- Add SPF and DKIM records to DNS
- Use a reputable SMTP service (SendGrid, Mailgun)

### Problem: No error but email not received

**Solutions**:
- Check spam/junk folder
- Verify recipient email is correct
- Check SMTP server logs
- Test with different recipient email

## Security Best Practices

### ⚠️ DO NOT commit passwords to source control!

**Option 1: Use appSettings (for development)**

In `Web.config`:
```xml
<appSettings>
  <add key="SmtpHost" value="smtp.gmail.com" />
  <add key="SmtpPort" value="587" />
  <add key="SmtpUser" value="library@gmail.com" />
  <add key="SmtpPassword" value="your-app-password" />
</appSettings>
```

Then modify `EmailHelper.cs` to read from appSettings instead of system.net/mailSettings.

**Option 2: Environment Variables (for production)**

Set environment variables on the server and read them in code.

**Option 3: Azure Key Vault (for Azure deployments)**

Store secrets in Key Vault and retrieve at runtime.

## Production Recommendations

1. **Use a dedicated email service**: SendGrid, Mailgun, AWS SES
2. **Implement retry logic**: Handle transient failures
3. **Add email queue**: Don't block HTTP requests
4. **Log email attempts**: Track success/failure
5. **Rate limiting**: Prevent abuse
6. **Email templates**: Store in database for easy updates
7. **Unsubscribe links**: For compliance (if needed)

## Email Template Customization

The current email template is in `EmailHelper.cs`. To customize:

1. Open `Helpers/EmailHelper.cs`
2. Find the `SendPasswordResetEmail` method
3. Modify the HTML in the `body` variable
4. Update colors, text, logo URL, etc.

Example customization:
```csharp
string body = $@"
    <html>
    <body style='font-family: Arial;'>
        <img src='YOUR_LOGO_URL' alt='Logo' style='width: 200px;' />
        <h2>Reset Your Password</h2>
        <p>Hi {userName},</p>
        <p>Click below to reset your password:</p>
        <a href='{resetLink}' style='background: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
            Reset Password
        </a>
        <p>Link expires in 24 hours.</p>
    </body>
    </html>
";
```

## Summary

✅ Configure SMTP in `Web.config`  
✅ Use Gmail App Password for testing  
✅ Use SendGrid for production  
✅ Never commit passwords  
✅ Test email functionality before deploying  

Once configured, the password reset feature will work automatically!
