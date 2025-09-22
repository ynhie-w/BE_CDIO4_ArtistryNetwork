using CDIO4_BE.Domain.Entities;
using CDIO4_BE.Domain.DTOs;

namespace CDIO4_BE.Services.Interfaces
{

public interface INguoiDungService
{
    Task<IEnumerable<NguoiDung>> LayTatCaNguoiDung();
    Task<bool> CapNhatNguoiDung(int id, CapNhatNguoiDungDto dto);
    Task<bool> XoaNguoiDung(int id);
    }
}
