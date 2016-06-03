var express = require('express');
var router = express.Router();
var ordercontroller = require('../controllers/orderscontroller');

isAuthenticated = function (req, res, next) {
	// if user is authenticated in the session, call the next() to call the next request handler 
	// Passport adds this method to request object. A middleware is allowed to add properties to
	// request and response objects
	if (req.isAuthenticated())
		return next();
	// if the user is not authenticated then redirect him to the login page
	res.redirect('/');
}
/* GET home page. */
router.get('/', function(req, res, next) {
  res.render('index', { title: 'ShopNonStop' ,flash: req.flash('message'),isauth: req.isAuthenticated(),user: req.user});
});

router.get('/execute',function(req,res,next){
	ordercontroller.execute(req,res,next);
});

router.get('/aboutus',function(req,res,next){
	res.render('aboutus',{isauth:req.isAuthenticated(),user:req.user});
});

module.exports = router;
