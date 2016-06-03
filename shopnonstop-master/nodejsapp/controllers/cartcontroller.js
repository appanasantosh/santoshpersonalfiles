var Cart = require('../models/cart');
var Product = require('../models/productinformation');
var Address= require('../models/address');
var mongodb = require('mongodb');

var cart_id;

 function add(req,res,next){
 	var cart = new Cart;
 	cart.productid=req.body.productid;
 	cart.userid=req.user._id;
 	cart.quantity=req.body.quantity;
 	cart.isacart = '0';
 	cart.save(function(err,data){
 		if(err)
 		{
 			console.log(err);
 		}
 		else
 		{
 			cart_id = data._id;
 			res.send("productid:" +data.productid);
 		}
 	})
 }

 function addtocart(req,res,next)
 {
 	var cart = new Cart;
 	cart.productid = req.body.productid;
 	cart.userid = req.user._id;
 	cart.quantity = '0';
 	cart.isacart = '1';
 	cart.save(function(err,cart){
 		if(err)
 		{
 			return next(err);
 		}
 		else
 		{
 			Cart.count({isacart:'1',userid:req.user._id},function(err,count)
 			{
 				if(err)
 				{
 					return next(err);
 				}
 				else
 				{
 					res.send(JSON.stringify({number:count}));
 				}
 				
 			});
 		}
 	});
 }

function buy(req,res,next){
	console.log("cartid:"+cart_id);
	Cart.findOne({_id:cart_id}).lean().exec(function(err,carts){
		if(err)
		{
			console.log(err);
		}
		else
		{

			Product.findOne({_id:carts.productid}).lean().exec(function(err,products){
				if(err)
				{
					console.log(err);
				}
				else
				{
					Address.find({userid:req.user._id}).lean().exec(function(err,address){
						if(err)
						{
							console.log(err);
						}
						else
						{
							res.render('carts/buy',{isauth:req.isAuthenticated(),user: req.user,cart:carts,product:products,address:address});
						}
					});
					
				}
			});
		}
	});
}

function shippingaddress(req,res,next)
{
	Cart.update({_id:new mongodb.ObjectID(req.body.cartid)},{addressid:new mongodb.ObjectID(req.body.addressid)},function(err,cart){
		if(err)
		{
			console.log(err);
		}
		else
		{
			res.end(JSON.stringify(cart));
		}
		cart_id= req.body.cartid;
	});
}
function paymentoptions(req,res,next)
{
	Cart.findOne({_id:cart_id}).lean().exec(function(err,data){
		if(err)
		{
			console.log(err);
		}
		Address.findOne({_id:data.addressid}).lean().exec(function(err,address)
		{
			if(err)
			{
				console.log(err);
			}
			else
			{
				Product.findOne({_id:data.productid}).lean().exec(function(err,product){
					if(err)
					{
						console.log(err);
					}
					res.render('carts/paymentoptions',{isauth:req.isAuthenticated(),user:req.user,cart:data,address:address,product:product});
				})
				
			}

		});
			
	});

}

function usercarts(req,res,next)
{
	Cart.find({isacart:1,userid:req.user._id}).lean().populate('productid').exec(function(err,usercarts){
		if(err)
		{
			return next(err);
		}
		else
		{
			console.log(usercarts);
			res.render('carts/cartdata',{isauth:req.isAuthenticated(),user:req.user,cartsdata:usercarts});
		}
	});
}

function removecart(req,res,next)
{
	Cart.update({userid:req.user._id,_id:req.body.remcartid},{isacart:'0'},function(err,cart){
		if(err)
		{
			return next(err);
		}
		else
		{
			res.end(JSON.stringify(cart));
		}
	});
}
 module.exports.add = add;
 module.exports.buy = buy;
 module.exports.shippingaddress = shippingaddress;
 module.exports.paymentoptions = paymentoptions;
 module.exports.addtocart = addtocart;
module.exports.usercarts = usercarts;
module.exports.removecart = removecart;