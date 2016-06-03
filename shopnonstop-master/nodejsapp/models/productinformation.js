
var mongoose = require('mongoose');
var uniqueValidator = require('mongoose-unique-validator');
var Schema = mongoose.Schema;


var ProductSchema = new Schema({category:{type: String, default: ''}, 
	sellerinfo:{type: String},productdesc:{type: String, default: ''},
	userid:{type: Schema.Types.ObjectId},imgpath:{type: String},price:{type: Number},brand:{type: String},model:{type: String}});




module.exports = mongoose.model('Product',ProductSchema);