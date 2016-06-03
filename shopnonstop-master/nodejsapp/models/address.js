
var mongoose = require('mongoose');
var uniqueValidator = require('mongoose-unique-validator');
var Schema = mongoose.Schema;


var AddressSchema = new Schema({userid:{type:Schema.Types.ObjectId},street:{type: String},
	landmark:{type: String},city:{type: String},state:{type: String},country:{type:String},pincode:{type: String},profileaddress:{type:Number, default:'0'},defaultaddress:{type:Number,default:'0'}});



module.exports = mongoose.model('Address',AddressSchema);