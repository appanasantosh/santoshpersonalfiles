
var mongoose = require('mongoose');
var uniqueValidator = require('mongoose-unique-validator');
var Schema = mongoose.Schema;

var CartSchema= new Schema({productid:{type: Schema.Types.ObjectId,ref:'Product'},userid:{type: Schema.Types.ObjectId},quantity:{type: Number},addressid:{type: Schema.Types.ObjectId},isacart:{type:Number}})




module.exports = mongoose.model('Cart',CartSchema);