var paypal = require('paypal-rest-sdk');

function init(config)
{
  var configuration = config;
  paypal.configure(configuration.api);
}

function buy(req,res,next)
{
  console.log("buy....................................................");

            var payment={"intent":"sale",
            "payer":{"payment_method":"paypal"},
            "redirect_urls":{"return_url":"http://localhost:8080/execute","cancel_url":"http://localhost:8080/"},
            "transactions": [{
                      "amount": {
                        "total": req.body.price,
                        "currency": "USD"
                      },
                      "description": req.body.productdesc
                      }]};
  paypal.payment.create(payment,function(err,paymentdata)
  {
    if(err)
    {
      console.log(err);
      next(err);
    }
    else
    {
      if(payment.payer.payment_method === 'paypal')
      {
      for(var i=0; i < paymentdata.links.length; i++) 
        {
            var link = paymentdata.links[i];
             if (link.method === 'REDIRECT') 
              {
                 redirectUrl = link.href;
              }
        }
        var paypalurl = {redirecturl:redirectUrl};
        res.end(JSON.stringify(paypalurl));
      }
    }
  });                 

}

function execute(req,res,next)
{
  var paymentid = req.param('paymentId');
  var details = {"payer_id":req.param('PayerID')};
  paypal.payment.execute(paymentid,details,function(err,paymentdata)
  {
    if(err)
    {
       next(err);
    }
    else
    {
      console.log(paymentdata);
      res.redirect('/transactionstatus');
    }
  })
}


module.exports.init = init; 
module.exports.buy = buy;
module.exports.execute = execute;
