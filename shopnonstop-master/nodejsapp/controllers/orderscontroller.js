var User = require('../models/users');
var Profile = require('../models/profile');
var Address = require('../models/address');
var Cart = require('../models/cart');
var Order = require('../models/orders');
var twilio = require('twilio')('ACa4c57479fdc5189ebf84c87378d9e14f','a800da65bae1fe9836eacac320a35860');
var mongodb = require('mongodb');
var paypal = require('paypal-rest-sdk');
var config = {};
var nodemailer = require('nodemailer');
var password = '9030476311'
var Smtpconfig = {host: 'smtp.gmail.com',port:465,secure:true,auth:{user:'jimmyappana@gmail.com',pass:password}};
var format = require('date-format-lite');


function init(c)
{
	config = c;
	paypal.configure(c.api);
	
}
var order_id;
function pay(req,res,next)
{
	var now = new Date();
	var order = new Order;
	order.userid=req.user._id;
	order.cartid=req.body.cart_id;
	order.amount = req.body.amount;
	order.paymode = req.body.mode;
	order.ordereddate = now.format('DDD, D MMM-YY');
	order.created = now;
	order.save(function(err,data){
		if(err)
		{
			return console.log(err);
		}
		else
		{
			Cart.update({userid:req.user._id,_id:new mongodb.ObjectID(req.body.cart_id)},{isacart:'0'},{multi: true},function(err,cartdata){
				if(err)
				{
					return next(err);
				}
					
			order_id = data._id;
			Profile.findOne({userid:req.user._id},function(err,data){
				if(err)
				{
					console.log(err);
				}
				else
				{
					if(req.body.mode == 'COD')
					{
						var mobilenum='+91'+data.mobile;
						twilio.sendMessage({
						to: mobilenum, // the number for the phone in your pocket
						from:'+16073912274', // your Twilio number
						body: 'Thanks for your ShopNonStop order.'+'\n'+'Your order '+req.body.ordername+' has been confirmed with COD option. Please keep Rs.'+req.body.amount+' at the time of delivary'// The body of the text message
						}, function(error, message) {
						// This callback is executed when the request completes
						if (error) {
							console.error('We couldn\'t send the message');
						} else {
							console.log('Message sent! Message id: '+message.sid);
						}
						var mailoptions = {from:'jimmyappana@gmail.com',to:req.user.email,subject:'ShopNonStop payment details',html:'<head><link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css" ><script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js"></script></head>'+
						'<body><h1 style="font-family:ar destine">Shop Non Stop</h1></br>'+
						'Dear '+req.user.username+',</br>'+
						'<p>Greetings from ShopNonStop!</p>'+
						'<p>We are pleased to inform you that following items in your Order '+order_id+' have been packed by the seller and are ready to be shipped.</p></br>'+
						'<p>Items details:</p>'+
						'<table class="table table-bordered"><thead><tr><th>Product</th><th>Total</th></tr><thead><tbody><tr><td><a href="http://localhost:8080/users/show/'+req.body.productid+'">'+req.body.ordername+'</a></td><td>'+req.body.amount+'</td></tr></tbody></table></body>',
						attachments:[{filename:'sns.png',path:'https://localhost:8080/images/sns.png',cid:'@sns'}]};
						var Transporter = nodemailer.createTransport(Smtpconfig);
					   
						Transporter.sendMail(mailoptions,function(err,info){
							console.log('mail sent successfully',info.response);
						})
						res.end(JSON.stringify(data));
					});
					}
					else
					{
						var payment={"intent":"sale",
						"payer":{"payment_method":"paypal"},
						"redirect_urls":{"return_url":"https://localhost:8080/execute","cancel_url":"https://localhost:8080/"},
						"transactions": [{
											"amount": {
											  "total": req.body.amount,
											  "currency": "USD"
											},
											"description": req.body.ordername+" order payment."
										  }]};
						
						paypal.payment.create(payment,function(err,paymentdata)
						{
							if(err)
							{
								console.log(err);
							}
							else
							{
								console.log(paymentdata);
								if(payment.payer.payment_method === 'paypal')
								{
									req.session.paymentId = paymentdata.id;
									 for(var i=0; i < paymentdata.links.length; i++) {
											 var link = paymentdata.links[i];
										if (link.method === 'REDIRECT') {
										  redirectUrl = link.href;
										}
									  }
									  console.log(redirectUrl+'redirect url');
									  var mobilenum='+91'+data.mobile;
										twilio.sendMessage({
										to: mobilenum, // the number for the phone in your pocket
										from:'+16073912274', // your Twilio number
										body: 'Thanks for your ShopNonStop order.'+'\n'+'Your order '+req.body.ordername+' has been confirmed with PayPal option.'// The body of the text message
										}, function(error, message) {
										// This callback is executed when the request completes
										if (error) {
											console.error('We couldn\'t send the message');
										} else {
											console.log('Message sent! Message id: '+message.sid);
										}
									   
									});
									  var url ={rurl:redirectUrl};
									  res.end(JSON.stringify(url));
								}
							}
						});
					}
					
				}
			});
			
			});
		}
	});
}

function execute(req,res,next)
{
	var paymentid = req.session.paymentId;
	var payerid = req.param('PayerID');
	var details = {"payer_id":payerid};
	paypal.payment.execute(paymentid,details,function(err,payment){
		if(err)
		{
			console.log(err);
		}
		else
		{
			Order.update({_id: order_id},{paymentstatus:'1'},function(err,data)
			{
				if(err)
				{
					console.log(err);
				}
				else
				{
					res.redirect('/users/paymentstatus');
				}
			});
			
		}
	});
}

function paymentstatus(req,res,next)
{
	if(req.isAuthenticated())
	{
		res.render('carts/paymentstatus',{paymentid:req.session.paymentId,isauth: req.isAuthenticated(),user:req.user});
	}
	else
	{
		res.redirect('/');
	}
	
}

module.exports.pay = pay;
module.exports.init = init
module.exports.execute = execute;
module.exports.paymentstatus = paymentstatus;