
var mongoose = require('mongoose');
var uniqueValidator = require('mongoose-unique-validator');
var Schema = mongoose.Schema;


var UserSchema = new Schema({username:{type: String, default: ''}, 
	email:{type: String, unique: true, required: true},password:{type: String, default: ''}});
UserSchema.plugin(uniqueValidator);

UserSchema.statics = {search: function(email){
	return this.findOne(email);
}
};

module.exports = mongoose.model('User',UserSchema);