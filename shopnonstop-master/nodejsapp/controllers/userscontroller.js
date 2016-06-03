var User = require('../models/users');
var Profile = require('../models/profile')
var Address = require('../models/address')
var Orders = require('../models/orders');
var mongodb = require('mongodb');
var nodemailer = require('nodemailer');
var password = '9030476311';
var smtpconfig={host: 'smtp.gmail.com',port:465,secure:true,auth:{user:'jimmyappana@gmail.com',pass:password}}
var transporter = nodemailer.createTransport(smtpconfig);
 function register(req,res,next){
	
	var newuser = new User({username: req.body.username, email: req.body.email, password: req.body.userpassword});
	newuser.save(function(err,data){
		if(err)
		{
			//req.flash("messages",{"error": err});
			res.locals.messages = req.flash('message',err.message);
			res.render('index',{title:'Express',flash: req.flash('message')});
		}
		else
		{
			var mailoptions={from:'jimmyappana@gmail.com',to:req.body.email,subject:'ShopNonStop registration',html:'<body><h1 style="font-family:ar destine">Shop Non Stop</h1></br>'+
						'<p>Thanks for your registering on ShopNonStop.</p></br><p>Here you can find range of products for low rates. So hurry up!</p></body>',
						attachments:[{filename:'sns.png',path:'http://localhost:8080/images/sns.png',cid:'@sns'}]};
			transporter.sendMail(mailoptions,function(err,info)
			{
				if(err)
				{
					console.log(err);
				}
				else
				{
					console.log('mail sent successfully');

				}
			});		
			res.send(data.username +'has registered Successfully');
		}
	});

}

function logout(req,res,next){
	req.logout();
	res.redirect('/');
}

function sell(req,res,next){
	if(req.isAuthenticated() == true)
	{
		res.render('users/sell',{isauth: req.isAuthenticated(),user: req.user});
	}
	else
	{
		res.redirect('/');
	}
}

function userprofile(req,res,next)
{
	if(req.isAuthenticated() == true)
	{

		Profile.findOne({userid:req.user._id}).lean().exec(function(err,userprofile){
			if(err)
			{
				console.log(err);

			}
			else
			{
				Address.find({userid:req.user._id}).lean().exec(function(err,address)
				{
					if(err)
					{
						console.log(err);
					}
					else
					{
						res.render('users/profile',{isauth: req.isAuthenticated(),user:req.user, profile:userprofile, addresses:address});
					}
				});
				
			}
		});
		//res.render('users/profile',{isauth: req.isAuthenticated(),user:req.user});

	}
	else
	{
		res.redirect('/');
	}
}

function createprofile(req,res,next)
{
	Profile.findOne({userid: req.user._id},function(err,profile){
		if(err)
		{
			console.log(err);
		}
		if(!profile)
		{
			var profile = new Profile;
			profile.name = req.body.fullname;
			profile.email = req.body.email;
			profile.mobile = req.body.mobile;
			profile.gender = req.body.gender;
			profile.street = req.body.street;
			profile.landmark = req.body.landmark;
			profile.city = req.body.city;
			profile.state = req.body.state;
			profile.country = req.body.country;
			profile.pincode = req.body.pincode;
			profile.userid = req.user._id;
			profile.save(function(err,data){
				if(err)
				{
					console.log(err);
				}
				else
				{
					var address = new Address;
					address.street = req.body.street;
					address.landmark = req.body.landmark;
					address.city = req.body.city;
					address.state = req.body.state;
					address.country = req.body.country;
					address.pincode = req.body.pincode;
					address.userid = req.user._id;
					address.profileaddress = '1';
					address.defaultaddress = '1';
					address.save(function(err,address){
						if(err)
						{
							console.log(err);
						}
					});
					res.locals.messages = req.flash('message','Profile updated successfully.')
					Address.find({userid:req.user._id}).lean().exec(function(err,address)
					{
						if(err)
						{
							console.log(err);
						}
						else
						{
							res.render('users/profile',{isauth: req.isAuthenticated(),user:req.user,message:req.flash('message'),profile:data,addresses:address});
						}
					});
					
				}
			});
		}
		else
		{
			Profile.update({userid:req.user._id},{name:req.body.fullname,email:req.body.email,mobile:req.body.mobile,gender:req.body.gender,
				street:req.body.street,landmark:req.body.landmark,city:req.body.city,state:req.body.state,country:req.body.country,
				pincode:req.body.pincode},function(err,data){
					if(err)
					{
						console.log(err);
					}
					else
					{
						Address.update({userid:req.user._id,profileaddress: '1'},{street:req.body.street,landmark:req.body.landmark,city:req.body.city,state:req.body.state,country:req.body.country,
						pincode:req.body.pincode},function(err,address){
							if(err)
							{
								console.log(err);
							}
						});
						Profile.findOne({userid: req.user._id}).lean().exec(function(err,data){
							Address.find({userid:req.user._id}).lean().exec(function(err,address)
							{
								if(err)
								{
									console.log(err);
								}
								else
								{
									res.render('users/profile',{isauth: req.isAuthenticated(),user:req.user,message:req.flash('message'),profile:data,addresses:address});
								}
							});
							//res.render('users/profile',{isauth: req.isAuthenticated(),user:req.user,message:req.flash('message'),profile:data});
						});
						
					}
				});
		}
	});
}

function saveaddress(req,res,next){
	var address = new Address;
					address.street = req.body.street;
					address.landmark = req.body.landmark;
					address.city = req.body.city;
					address.state = req.body.state;
					address.country = req.body.country;
					address.pincode = req.body.pincode;
					address.userid = req.user._id;
					address.profileaddress='0';
					address.defaultaddress='0';
					address.save(function(err,address){
						if(err)
						{
							return next(err);
						}
						else
						{
							
							res.end(JSON.stringify(address));
						}
					});
}

function deleteaddress(req,res,next)
{
	Address.remove({_id:new mongodb.ObjectID(req.body.addrsid)},function(err,data){
		console.log(data);
		res.end(JSON.stringify(data));
	});
}

function defaultaddress(req,res,next)
{
	Address.update({userid:req.user._id},{defaultaddress:'0'},{multi: true},function(err,address){
		if(err)
		{
			console.log(err);
		}
		else
		{
			console.log(address);
			Address.update({_id:new mongodb.ObjectID(req.body.address_id)},{defaultaddress:'1'},function(err,data){
				if(err)
				{
					console.log(err);
				}
				else
				{
					console.log(data);
					res.end(JSON.stringify(data));
				}
			});
		}
	});
	
}

function userorders(req,res,next)
{
	//Orders.find({userid:req.user._id}).lean().populate('cartid').exec(function(err,ordersdata){
	Orders.find({userid:req.user._id}).sort({created: -1}).deepPopulate('cartid.productid').lean().exec(function(err,ordersdata){
		if(err)
		{
			console.log(err);
			return next(err);
		}
		else
		{
			console.log(ordersdata);
			res.render('users/orders',{isauth:req.isAuthenticated(),user:req.user,orders:ordersdata});
		}
	});
}

module.exports.register = register;
module.exports.logout = logout;
module.exports.sell = sell; 
module.exports.userprofile = userprofile;
module.exports.createprofile = createprofile;
module.exports.saveaddress = saveaddress;
module.exports.deleteaddress = deleteaddress;
module.exports.defaultaddress = defaultaddress;
module.exports.userorders = userorders; 