var express = require('express');
var router = express.Router();
var controller = require('../controllers/userscontroller');
var productcontroller = require('../controllers/productinfocontroller');
var cartcontroller = require('../controllers/cartcontroller');
var ordercontroller = require('../controllers/orderscontroller');


var isAuthenticated = function (req, res, next) {
	// if user is authenticated in the session, call the next() to call the next request handler 
	// Passport adds this method to request object. A middleware is allowed to add properties to
	// request and response objects
	if (req.isAuthenticated())
		return next();
	// if the user is not authenticated then redirect him to the login page
	res.redirect('/');
}


/* GET users listing. */
module.exports = function(passport)
{
router.get('/', function(req, res, next) {
  res.send('respond with a resource');
});

router.post('/register', passport.authenticate('signup',{successRedirect: '/',failureRedirect:'/',failureflash:true}))
	//controller.register(req,res,next);

router.get('/test',function(req,res,next){
	res.render('test');
});
router.post('/login',passport.authenticate('login',{successRedirect: '/',failureRedirect:'/',failureflash:true}));

router.get('/logout',function(req,res,next)
{
	controller.logout(req,res,next);
});

router.get('/sell',function(req,res,next)
{
	controller.sell(req,res,next);
});

router.post('/createsell',function(req,res,next){
	if(req.isAuthenticated() == true)
	{
		productcontroller.createsell(req,res,next);
	}
	else
	{
		res.redirect('/');
	}

});

router.post('/products',function(req,res,next){
	productcontroller.products(req,res,next,req.body.category);
	console.log(req.body.category);
});

router.get('/product',function(req,res,next){
	productcontroller.product(req,res,next);
})
router.get('/profile',function(req,res,next){
	controller.userprofile(req,res,next);
});

router.post('/createprofile',function(req,res,next){
	if(req.isAuthenticated() == true)
	{
		controller.createprofile(req,res,next);
	}
	else
	{
		res.redirect('/');
	}
});

router.post('/productdetails',function(req,res,next){
	productcontroller.productdetails(req,res,next)
});
router.get('/show',function(req,res,next){
	productcontroller.show(req,res,next);
});

router.get('/show/:orderid',function(req,res,next)
{
	productcontroller.showonid(req,res,next);
});

router.post('/buynow',function(req,res,next){
	cartcontroller.add(req,res,next);
});

router.post('/addtocart',function(req,res,next){
	cartcontroller.addtocart(req,res,next);
});

router.get('/buy',function(req,res,next){
	if(req.isAuthenticated() == true)
	{
		cartcontroller.buy(req,res,next);
	}
	else
	{
		res.redirect('/');
	}
});

router.post('/saveaddress',function(req,res,next){
	controller.saveaddress(req,res,next);
});

router.post('/deleteaddress',function(req,res,next){
	controller.deleteaddress(req,res,next);
});

router.post('/defaultaddress',function(req,res,next)
{
	controller.defaultaddress(req,res,next);
});

router.post('/shippingaddress',function(req,res,next)
{
	cartcontroller.shippingaddress(req,res,next);
});

router.get('/paymentoptions',function(req,res,next){
	if(req.isAuthenticated() == true)
	{
		cartcontroller.paymentoptions(req,res,next);
	}
	else
	{
		res.redirect('/');
	}
});

router.post('/pay',function(req,res,next){
	ordercontroller.pay(req,res,next);
});



router.get('/paymentstatus',function(req,res,next)
{
	ordercontroller.paymentstatus(req,res,next);
});

router.get('/orders',function(req,res,next){
	if(req.isAuthenticated() == true)
	{
		controller.userorders(req,res,next);
	}
	else
	{
		res.redirect('/');
	}
});

router.get('/usercarts',function(req,res,next){
	if(req.isAuthenticated())
	{
		cartcontroller.usercarts(req,res,next);
	}
	else
	{
		res.redirect('/');
	}
});

router.post('/removecart',function(req,res,next)
{
	if(req.isAuthenticated())
	{
		cartcontroller.removecart(req,res,next);
	}
	else
	{
		res.redirect('/');
	}
});

return router;
//module.exports = router;
}