var express = require('express');
var router = express.Router();
var buycontroller = require('../controllers/buycontroller');

/* GET home page. */
router.get('/', function(req, res, next) {
  res.render('index', { title: 'Paypal Integration' });
});

router.post('/buy',function(req,res,next){
	buycontroller.buy(req,res,next);
});

router.get('/execute',function(req,res,next){
	buycontroller.execute(req,res,next);
});

router.get('/transactionstatus',function(req,res,next){
	res.render('status');
});

module.exports = router;
