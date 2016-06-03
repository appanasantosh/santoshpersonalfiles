class AddColumnDoTodayToGoal < ActiveRecord::Migration
  def change
    add_column :goals, :DoToday, :datetime, :null => false
  end
end
