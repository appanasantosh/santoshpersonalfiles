var express = require('express');
var fs = require('fs');
var path = require('path');
var favicon = require('serve-favicon');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var passport = require('passport');
var session = require('express-session');
var routes = require('./routes/index');
var users = require('./routes/users');
var ejslayouts = require('express-ejs-layouts');
var app = express();
var mongoose = require('mongoose');
var flash = require('connect-flash')
var multer = require('multer');
var upload = multer({dest:'./public/images/uploads'});
var ordercontroller = require('./controllers/orderscontroller');
var Cart = require('./models/cart');
try {
  var configJSON = fs.readFileSync(__dirname + "/passport/paypalconfig.json");
  var config = JSON.parse(configJSON.toString());
} catch (e) {
  console.error("File config.json not found or is invalid: " + e.message);
  process.exit(1);
}
ordercontroller.init(config);

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'ejs');
//app.set('layout','../views/layout');

// uncomment after placing your favicon in /public
//app.use(favicon(path.join(__dirname, 'public', 'favicon.ico')));
app.use(multer({dest:'./public/images/uploads'}).single('productimg'))
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(session({cookie: { maxAge: 24 * 60 * 60 * 1000 },secret: 'shopnonstop',resave: false, 
                  saveUninitialized: false}));
app.use(flash());
app.use(passport.initialize());
app.use(passport.session());
var localpassport = require('./passport/init');
localpassport(passport);
app.use(express.static(path.join(__dirname, 'public')));
app.use(ejslayouts);
app.use(function(req,res,next){
  console.log('........................................');
  if(req.isAuthenticated())
  {
     Cart.count({isacart:'1',userid:req.user._id},function(err,count){
    if(err)
    {
      next(err);
      
    }
    else
    {
      console.log('........................................');
      app.locals.count = count;
      next();
    }
  });
  }
  else
  {
    next();
  }
});
var users = require('./routes/users')(passport);
app.use('/', routes);
app.use('/users', users);

app.use(function(req, res, next) {
  //res.locals.messages = req.flash();
  next();
});

// catch 404 and forward to error handler
app.use(function(req, res, next) {
  var err = new Error('Not Found');
  err.status = 404;
  next(err);
});

// error handlers

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
  mongoose.connect("mongodb://localhost/shopping",function(err){
    if(err)
    {
      console.log(err);
    }
    else
    {
      console.log('Connection established to mongodb');
    }
  });
  app.use(function(err, req, res, next) {
    res.status(err.status || 500);
    res.render('error', {
      message: err.message,
      error: err
    });
  });
}

// production error handler
// no stacktraces leaked to user
if(app.get('env') === 'production')
{
  mongoose.connect("mongodb://santosh:123456@ds061335.mongolab.com:61335/shopnonstop",function(err){
    if(err)
    {
      console.log(err);
    }
    else
    {
      console.log('Connection established to mongodb');
    }
  });

app.use(function(err, req, res, next) {
  res.status(err.status || 500);
  res.render('error', {
    message: err.message,
    error: {}
  });
});

}




module.exports = app;
