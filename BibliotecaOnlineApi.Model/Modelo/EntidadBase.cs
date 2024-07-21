using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaOnlineApi.Model.Modelo
{
    public class EntidadBase<Tid> : ICloneable
    {
        private int? _solicitudHashCode;

        [Key]
        [Column("ID")]
        public virtual Tid Id { get; set; }

        [Column("FECHA_CREACION")]
        [DataType(DataType.DateTime)]
        public virtual DateTime FechaCreacion { get; set; }

        [Column("USUARIO_CREACION")]
        [DataType(DataType.Text)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe ingresar el usuario de creación")]
        [MaxLength(250, ErrorMessage = "El valor ingresado supera los 250 caracteres.")]
        public virtual string UsuarioCreacion { get; set; }

        [Column("FECHA_ACTUALIZACION")]
        [DataType(DataType.DateTime)]
        public virtual DateTime? FechaActualizacion { get; set; }

        [Column("USUARIO_ACTUALIZACION")]
        [DataType(DataType.Text)]
        [MaxLength(250, ErrorMessage = "El valor ingresado supera los 250 caracteres.")]
        public virtual string UsuarioActualizacion { get; set; }

        [Column("REGISTRO_ELIMINADO")]
        public virtual bool Eliminado { get; set; }

        [Column("FECHA_ELIMINACION")]
        [DataType(DataType.DateTime)]
        public virtual DateTime? FechaEliminacion { get; set; }

        [Column("USUARIO_ELIMINACION")]
        [DataType(DataType.Text)]
        [MaxLength(250, ErrorMessage = "El valor ingresado supera los 250 caracteres.")]
        public virtual string UsuarioEliminacion { get; set; }



        //METODOS
        public bool EsTransitorio()
        {
            return Id.Equals(Guid.Empty);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is EntidadBase<Tid>))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            EntidadBase<Tid> other = (EntidadBase<Tid>)obj;
            return !EsTransitorio() && !other.EsTransitorio() && Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            if (!EsTransitorio())
            {
                if (!_solicitudHashCode.HasValue)
                {
                    _solicitudHashCode = Id.GetHashCode() ^ 0x1F;
                }

                return _solicitudHashCode.Value;
            }

            return base.GetHashCode();
        }
        public object Clone()
        {

            return MemberwiseClone();
        }



        //OPERADORES
        public static bool operator ==(EntidadBase<Tid> left, EntidadBase<Tid> right)
        {
            if (Equals(left, null))
            {
                if (!Equals(right, null))
                {
                    return false;
                }
                return true;
            }
            return left.Equals(right);
        }

        public static bool operator !=(EntidadBase<Tid> left, EntidadBase<Tid> right)
        {
            return !(left == right);
        }

    }
}
