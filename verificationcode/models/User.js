var mongoose = require('mongoose');
var uniqueValidator = require('mongoose-unique-validator');
var config = require('../config');
var bcrypt = require('bcrypt');
var authy = require('authy')(config.authykey);
var twilio = require('twilio')(config.accountSid,config.authToken);
var SALT_WORK_FACTOR = 10;


var UserSchema = new mongoose.Schema({
	name:{type: String,required: true},
	countryCode:{type: String, required: true},
	phone:{type: String, required: true},
	verified:{type: Boolean, default: false},
	authyid:{type: String},
	email:{type: String, required: true, unique: true},
	password:{type: String, required: true}
});
UserSchema.plugin(uniqueValidator);

UserSchema.pre('save',function(next)
{
	var self = this;

	if(! self.isModified('password')) 	//for checking that password is modified or new
		{
			return next();
		}
	bcrypt.genSalt(SALT_WORK_FACTOR,function(err,salt)	// generates the salt
	{
		if(err)
		{
			return next(err);
		}
		bcrypt.hash(self.password,salt,function(err,hash) // converts the password to hash by using salt 
		{
			if(err)
			{
				return next(err);
			}
			self.password = hash;						// save this hash password in db.
			next();
		});

	});

});

UserSchema.methods.comparePassword = function(candidatePassword, cb){
	var self = this;
	bcrypt.compare(candidatePassword,self.password,function(err,ismatch){
		if(err)
		{
			cb(err);
		}
		cb(null,ismatch);
	});
}

UserSchema.methods.sendAuthyToken = function(cb){
	var self = this;
	if(!self.authyid)		// registering the new user in authy with authyid
	{
		authy.register_user(self.email,self.phone,self.countryCode,function(err,response)
		{
			if(err || !response.user)
			{
				cb.call(self,err);
			}
			console.log(response);
			self.authyid = response.user.id;
			self.save(function(err,data)
			{
				if(err || !data)
				{
					cb.call(self,err);
				}
				self = data;
				sendToken();
			});
		});
	}
	else
	{
		sendToken();
	}
	function sendToken()				// for sending 2FA token to the user registered
	{
		authy.request_sms(self.authyid,true,function(err,response)
		{
			console.log(response);
			cb.call(self,err);
		});
	}
}

UserSchema.methods.verifyAuthyToken = function(otp,cb)		// verifies the otp typed by the user with the otp which is sent to the user based on authyid.
{
	var self = this;
	authy.verify(self.authyid, otp,function(err,response){
		cb.call(self,err,response);
	})
}

UserSchema.methods.sendMessage = function(message,cb)
{
	var self = this;
    twilio.sendMessage({
        to: self.countryCode+self.phone,
        from: config.twilioNumber,
        body: message
    }, function(err, response) {
        cb.call(self, err);
    });
}

module.exports = mongoose.model('User', UserSchema); 
