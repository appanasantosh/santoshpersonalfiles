var User = require('../models/User');

function showCreate(req,res,next)
{
	console.log('--------------------------------');
	res.render('users/create',{title:'Create User Account',error:req.flash('errors')});
}

function create(req,res,next)
{
	var user = new User({name: req.body.username,countryCode: req.body.countryCode,phone: req.body.phone,email: req.body.email,
		password: req.body.password});
	user.save(function(err,data){
		if(err)
		{
			console.log(err);
            // To improve on this example, you should include a better
            // error message, especially around form field validation. But
            // for now, just indicate that the save operation failed
            req.flash('errors', 'There was a problem creating your'
                + ' account - note that all fields are required. Please'
                + ' double-check your input and try again.');

            res.redirect('/users/showCreate');
		}
		else
		{
			user.sendAuthyToken(function(err){
				if(err)
				{
					 req.flash('errors', 'There was a problem in sending your token.');
				}
				res.redirect('/users/'+data._id+'/verify');
			})
		}
	});
}


function showVerify(req,res,next)
{
	res.render('users/verify',{title:'Verify phone number',error:req.flash('errors'),success:req.flash('successes'),id:req.params.id});
}


function resend(req,res,next)
{
	User.findById(req.params.id,function(err,user)
	{
		if (err || !user) {
            return die('User not found for this ID.');
        }

        // If we find the user, let's send them a new code
        user.sendAuthyToken(postSend);
	});
	function postSend(err)
	{
		if(err)
		{
			return die('errors', 'There was a problem in sending code. Please try again');
		}
		req.flash('successes', 'Code re-sent!');
        res.redirect('/users/'+req.params.id+'/verify');

	}
	 function die(message) {
        req.flash('errors', message);
        res.redirect('/users/'+req.params.id+'/verify');
    }
}

function verify(req,res,next)
{
	  var user;

    // Load user model
    User.findById(req.params.id, function(err, doc) {
        if (err || !doc) {
            return die('User not found for this ID.');
        }

        // If we find the user, let's validate the token they entered
        user = doc;
        user.verifyAuthyToken(req.body.code, postVerify);
    });

    // Handle verification response
    function postVerify(err) {
        if (err) {
            return die('The token you entered was invalid - please retry.');
        }

        // If the token was valid, flip the bit to validate the user account
        user.verified = true;
        user.save(postSave);
    }

    // after we save the user, handle sending a confirmation
    function postSave(err) {
        if (err) {
            return die('There was a problem validating your account '
                + '- please enter your token again.');
        }

        // Send confirmation text message
        var message = 'You did it! Signup complete :)';
        user.sendMessage(message, function(err) {
            if (err) {
                req.flash('errors', 'You are signed up, but '
                    + 'we could not send you a message. Our bad :(');
            }

            // show success page
            req.flash('successes', message);
            res.redirect('/users/'+user._id+'/user');
        });
    }

    // respond with an error
    function die(message) {
        req.flash('errors', message);
        res.redirect('/users/'+req.params.id+'/verify');
    }
}
function showUser(req,res,next)
{
	User.findById({_id:req.params.id},function(err,user)
	{
		if(err)
		{
			console.log(err);
		}
		else
		{
			res.render('users/user',{user:user})
		}
	});
}

module.exports.showCreate = showCreate;
module.exports.create = create;
module.exports.showVerify = showVerify;
module.exports.resend = resend;
module.exports.verify = verify;
module.exports.showUser = showUser;