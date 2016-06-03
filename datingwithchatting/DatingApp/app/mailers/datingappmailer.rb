class Datingappmailer < ApplicationMailer
		default from:"appanasantosh"
	def sendmail user,mode,name,email
		@messagemode = mode 
		if @messagemode == 0
			@userdetails = user
			mail(to: user.email, subject: 'Welcome to Cupid')
		else
			@currentusername = user
			@name = name
			mail(to: email, subject: 'Welcome to Cupid')
		end
	end
end
