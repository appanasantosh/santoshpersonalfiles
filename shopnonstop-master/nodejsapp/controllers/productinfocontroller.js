var User = require('../models/users');
var Product = require('../models/productinformation');
var fs = require('fs');
var mongodb = require('mongodb');

function createsell(req,res,next)
{
	var user = req.user;
	var sell = new Product;
	sell.category = req.body.category;
	sell.sellerinfo = req.body.information;
	sell.productdesc = req.body.productdescription;
	//sell.productimg.data = fs.readFileSync(req.file.path);
	sell.imgpath = req.file.path;
	//sell.productimg.content = req.file.type;
	sell.userid = user._id;
	sell.price = req.body.price;
	sell.model = req.body.model;
	sell.brand = req.body.brand;
	sell.save(function(err,data)
	{
		if(err)
		{
			console.log(err);
		}
		else
		{
			console.log("Selected category is: "+data.category);
			
			res.locals.messages = req.flash('message','Product is ready to sell.')
			res.render('users/sell',{message:req.flash('message'),isauth: req.isAuthenticated(),user: req.user})
		}
	});
}
var category;
function products(req,res,next,selected){
Product.find({category: selected}).lean().exec(function(err,data){
	if(err)
	{
		console.log(err);
	}
	else
	{
		category = selected;
		var prod = JSON.stringify(data);
		//console.log(category);
		//res.render('users/shoes',{products: data,isauth: req.isAuthenticated(),user: req.user})
		//res.redirect('/users/shoes');
		res.send('success');
	}
});
}

function product(req,res,next){
	if(typeof category === 'undefined')
	{
		var path= req.path;
		console.log(path.replace('/',''));
		category = path.trim();
	}
	Product.find({category: category}).lean().exec(function(err,data){
	if(err)
	{
		console.log(err);
	}
	else
	{
	
		var prod = JSON.stringify(data);
		//console.log(category);
		console.log("data"+data);
		res.render('users/shoes',{products: data,isauth: req.isAuthenticated(),user: req.user})
	}
});
}
var prodid
function productdetails(req,res,next){
 prodid = req.body.productid;
 res.send('success'+prodid);
}

function show(req,res,next){
	Product.findOne({_id: prodid}).lean().exec(function(err,data){
		if(err)
		{
			console.log(err);
		}
		else
		{
			if(data)
			{
				res.render('products/productdetails',{product: data,isauth: req.isAuthenticated(),user: req.user});
			}
		}
	});
}

function showonid(req,res,next)
{
	Product.findOne({_id:new mongodb.ObjectID(req.params.orderid)}).lean().exec(function(err,data)
	{
		if(err)
		{
			console.log(err);
		}
		else
		{
			if(data)
			{

				res.render('products/productdetails',{product: data,isauth: req.isAuthenticated(),user: req.user});
			}
			
		}
	});
}
module.exports.createsell = createsell;
module.exports.products = products;
module.exports.product = product;
module.exports.productdetails = productdetails;
module.exports.show = show;
module.exports.showonid = showonid;