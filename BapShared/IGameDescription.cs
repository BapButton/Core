using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BapShared
{
    public interface IGameDescription
    {
        /// <summary>
        /// This is the type of the Blazor Component that should be dynamically loaded on the index page to start the game.
        /// </summary>
        public Type TypeOfInitialDisplayComponent { get; }
        /// <summary>
        /// Name of the game - Shown in the Menu
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Description of the Game - Also generally show on the menu.
        /// </summary>
        public string Description { get; }
        /// <summary>
        /// Unique Id of the game. This is used to storea and retrieve game specific data in the database. This should never change once the game is published.
        /// Simplest to just use  a Guid. Generate one any way that you want. I like https://wasteaguid.info/;
        /// </summary>
        public string UniqueId { get; }
        /// <summary>
        /// If you fix a major bug that was impacticing how scores were calulated or change how scores are genereated then it is hard compare old scores with new scores. 
        /// By changing this by default you will get a brand new score board so that scores will be base don the curent scoring model.
        /// </summary>
        public string ScoringModelVersion { get; }
    }
}
