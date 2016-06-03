var express = require('express');
var router = express.Router();
var users = require('../controllers/users');
/* GET users listing. */
router.get('/', function(req, res, next) {
  res.send('respond with a resource');
});

router.get('/showcreate',function(req,res,next)
{
	users.showCreate(req,res,next);
})
router.post('/create',function(req,res,next){
	users.create(req,res,next);
});

router.get('/:id/verify',function(req,res,next)
{
	users.showVerify(req,res,next);
});

router.post('/:id/verify',function(req,res,next)
{
	users.verify(req,res,next);
});

router.post('/:id/resend',function(req,res,next)
{
	users.resend(req,res,next);
});

router.get('/:id/user',function(req,res,next){
	users.showUser(req,res,next);
});
module.exports = router;
