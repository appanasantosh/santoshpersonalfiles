
var mongoose = require('mongoose');
var uniqueValidator = require('mongoose-unique-validator');
var Schema = mongoose.Schema;
var deepPopulate = require('mongoose-deep-populate')(mongoose); 


var OrderSchema = new Schema({userid:{type: Schema.Types.ObjectId}, 
	cartid:{type: Schema.Types.ObjectId,ref:'Cart'},amount:{type: Number},paymode:{type:String},paymentstatus:{type:Number, default:'0'},ordereddate:{type: String},created:{type:Date}});



OrderSchema.plugin(deepPopulate,{whitelist:['productid','cartid.productid']});
module.exports = mongoose.model('Order',OrderSchema);